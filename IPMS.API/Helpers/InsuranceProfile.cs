using AutoMapper;
using Core.Entities;
using IPMS.API.Dtos;

namespace IPMS.API.Helpers
{
    public class InsuranceProfile : Profile
    {
        public InsuranceProfile()
        {
            CreateMap<InsurancePolicy, InsurancePolicyDto>().ReverseMap();
            CreateMap<InsurancePolicy, InsurancePolicyToDisplay>().ReverseMap();
        }
    }
}
