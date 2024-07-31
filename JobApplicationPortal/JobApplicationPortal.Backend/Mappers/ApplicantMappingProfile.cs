using AutoMapper;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.DTOModels;
using JobApplicationPortal.Models.ViewModels;

namespace JobApplicationPortal.Backend.API.Mappers
{
    public class ApplicantMappingProfile : Profile
    {
        public ApplicantMappingProfile()
        {
            CreateMap<JobApplicants, JobApplicantDto>();

            CreateMap<JobApplicantDto, JobApplicants>()
            .ForMember(dest => dest.ResumeFileName, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.CertificationsFilesNames, opt => opt.MapFrom(src => new List<string>()));


            CreateMap<JobApplicants, JobApplicantDto>()
            .ForMember(dest => dest.Resume, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.Certifications, opt => opt.MapFrom(src => new List<string>()));

            CreateMap<JobApplicants, JobApplicantViewModel>();

        }
    }
}
