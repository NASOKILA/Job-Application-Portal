using AutoMapper;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.DTOModels;

namespace JobApplicationPortal.Backend.API.Mappers
{
    public class ApplicantMappingProfile : Profile
    {
        public ApplicantMappingProfile()
        {
            CreateMap<Applicant, ApplicantDto>();
            CreateMap<ApplicantDto, Applicant>();
        }
    }
}
