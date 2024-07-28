using AutoMapper;
using JobApplicationPortal.Models;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.DTOModels;

namespace JobApplicationPortal.Backend.API.Mappers
{
    public class ApplicantMappingProfile : Profile
    {
        public ApplicantMappingProfile()
        {
            CreateMap<JobApplicants, JobApplicantDto>();
            CreateMap<JobApplicantDto, JobApplicants>();

            CreateMap<JobApplicantModel, JobApplicants>()
            .ForMember(dest => dest.ResumeFileName, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.CertificationsFilesNames, opt => opt.MapFrom(src => new List<string>()));

            CreateMap<JobApplicants, JobApplicantModel>()
            .ForMember(dest => dest.Resume, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.Certifications, opt => opt.MapFrom(src => new List<string>()));
        }
    }
}
