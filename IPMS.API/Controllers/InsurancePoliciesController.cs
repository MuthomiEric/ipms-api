using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using IPMS.API.Dtos;
using Newtonsoft.Json;
using IPMS.API.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;

namespace IPMS.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InsurancePoliciesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly IRepository<InsurancePolicy> _repository;
        private readonly ILogger<InsurancePoliciesController> _logger;

        public InsurancePoliciesController(IRepository<InsurancePolicy> repository,
            IMapper mapper, ILogger<InsurancePoliciesController> logger,
            IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetPolicies([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { Message = "Page number and page size must be greater than zero." });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var cacheKey = $"policies:{userId}:page:{pageNumber}:size:{pageSize}";
                var cachedPolicies = await _cache.GetRecordAsync<string>(cacheKey);

                if (!string.IsNullOrEmpty(cachedPolicies))
                {
                    var cachedResult = JsonConvert.DeserializeObject<List<InsurancePolicyToDisplay>>(cachedPolicies);
                    return Ok(new
                    {
                        Data = cachedResult,
                        Pagination = new
                        {
                            TotalItems = cachedResult.Count,
                            PageNumber = pageNumber,
                            PageSize = pageSize
                        }
                    });
                }

                var totalPolicies = await _repository.CountAsync();
                var totalPages = (int)Math.Ceiling(totalPolicies / (double)pageSize);

                var policies = await _repository.GetPaginatedAsync((pageNumber - 1) * pageSize, pageSize);
                var policyDtos = _mapper.Map<List<InsurancePolicyToDisplay>>(policies);

                await _cache.SetRecordAsync(cacheKey, JsonConvert.SerializeObject(policyDtos), TimeSpan.FromMinutes(5));

                return Ok(new
                {
                    Data = policyDtos,
                    Pagination = new
                    {
                        TotalItems = totalPolicies,
                        TotalPages = totalPages,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(ex));

                return StatusCode(500, "We have encountered a problem while processing your request, try again later");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPolicy(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var cacheKey = $"policy:{userId}:{id}";
                var cachedPolicy = await _cache.GetRecordAsync<string>(cacheKey);
                if (!string.IsNullOrEmpty(cachedPolicy))
                {
                    var policyDto = JsonConvert.DeserializeObject<InsurancePolicyToDisplay>(cachedPolicy);
                    return Ok(policyDto);
                }

                var policy = await _repository.GetByIdAsync(id);
                if (policy == null)
                {
                    return NotFound(new { Message = "Policy not found" });
                }
                var policyDtoToCache = _mapper.Map<InsurancePolicyToDisplay>(policy);
                await _cache.SetRecordAsync(cacheKey, JsonConvert.SerializeObject(policyDtoToCache), TimeSpan.FromMinutes(5));
                return Ok(policyDtoToCache);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(ex));

                return StatusCode(500, "We have encountered a problem while processing your request, try again later");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePolicy([FromBody] InsurancePolicyDto policyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var policy = _mapper.Map<InsurancePolicy>(policyDto);
                policy.CreatedBy = userId!;
                policy.CreatedDate = DateTime.Now;
                policy.PolicyHolderName = User.FindFirst(ClaimTypes.Name)?.Value!;
                policy.PolicyNumber = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                await _repository.AddAsync(policy);
                await _cache.RemoveAsync($"policies:{userId}");
                return CreatedAtAction(nameof(GetPolicy), new { id = policy.Id }, _mapper.Map<InsurancePolicyToDisplay>(policy));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(ex));

                return StatusCode(500, "We have encountered a problem while processing your request, try again later");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePolicy(Guid id, [FromBody] InsurancePolicyDto policyDto)
        {
            try
            {
                var existingPolicy = await _repository.GetByIdAsync(id);
                if (existingPolicy == null)
                {
                    return NotFound(new { Message = "Policy not found" });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _mapper.Map(policyDto, existingPolicy);
                existingPolicy.ModifiedBy = userId;
                existingPolicy.ModifiedDate = DateTime.Now;
                await _repository.UpdateAsync(existingPolicy);

                await _cache.RemoveAsync($"policies:{userId}");
                await _cache.RemoveAsync($"policy:{userId}:{id}");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(ex));

                return StatusCode(500, "We have encountered a problem while processing your request, try again later");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePolicy(Guid id)
        {
            try
            {
                var policy = await _repository.GetByIdAsync(id);
                if (policy == null)
                {
                    return NotFound(new { Message = "Policy not found" });
                }
                await _repository.DeleteAsync(id);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _cache.RemoveAsync($"policies:{userId}");
                await _cache.RemoveAsync($"policy:{userId}:{id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(ex));

                return StatusCode(500, "We have encountered a problem while processing your request, try again later");
            }
        }
    }

}
