using Core.Entities;
using Core.Interfaces;
using IPMS.API.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IPMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<SystemUser> _userManager;
        private readonly SignInManager<SystemUser> _signInManager;

        public AuthController(
            ITokenService tokenService,
            ILogger<AuthController> logger,
            UserManager<SystemUser> userManager,
            SignInManager<SystemUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                SystemUser? user;

                user = await _userManager.FindByNameAsync(loginDto.UserName);

                if (user == null) return BadRequest("Invalid credentials");

                if (!user.IsActive) return Unauthorized("User not active");

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!result.Succeeded) return Unauthorized("Invalid credentials");

                var tokenResponse = new TokenResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = _tokenService.CreateToken(user)
                };

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(ex));

                return StatusCode(500, "We have encountered a problem while processing your request, try again later");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegistrationDto registerDto)
        {
            try
            {
                string? userName;
                userName = registerDto.Email;
                if (await _userManager.FindByNameAsync(userName) != null)
                    return BadRequest("User name already taken");

                var user = new SystemUser
                {
                    IsActive = true,
                    UserName = userName,
                    Email = registerDto.Email,
                    LastName = registerDto.LastName,
                    FirstName = registerDto.FirstName
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                    return BadRequest("Error creating user");

                var tokenResponse = new TokenResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Token = _tokenService.CreateToken(user)
                };

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, JsonConvert.SerializeObject(ex));

                return StatusCode(500, "We have encountered a problem while processing your request, try again later");
            }
        }

    }

}
