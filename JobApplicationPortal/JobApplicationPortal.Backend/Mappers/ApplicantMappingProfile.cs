﻿using AutoMapper;
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
            .ForMember(dest => dest.ResumeFilePath, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.CertificationsFilesPath, opt => opt.MapFrom(src => new List<string>()));
        }
    }
}
