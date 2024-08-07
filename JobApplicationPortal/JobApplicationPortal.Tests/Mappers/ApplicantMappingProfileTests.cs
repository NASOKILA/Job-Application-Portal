using AutoMapper;
using JobApplicationPortal.Backend.API.Mappers;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.DTOModels;
using JobApplicationPortal.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace JobApplicationPortal.Tests.Mappers
{
    public class ApplicantMappingProfileTests
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ApplicantMappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Test]
        public void ShouldMapJobApplicantsToJobApplicantDto()
        {
            // Arrange
            var jobApplicant = new JobApplicants
            {
                Id = new Guid("1f65ebc8-e773-43a9-b336-282dec4cde25"),
                Name = "John Wick",
                Email = "john.wick@example.com",
                Position = "Developer",
                ResumeFileName = "resume.pdf",
                CertificationsFilesNames = new List<string> { "cert1.pdf", "cert2.pdf" }
            };

            // Act
            var result = _mapper.Map<JobApplicantDto>(jobApplicant);

            // Assert
            Assert.AreEqual(jobApplicant.Name, result.Name);
            Assert.AreEqual(jobApplicant.Email, result.Email);
            Assert.AreEqual(jobApplicant.Position, result.Position);
        }

        [Test]
        public void ShouldMapJobApplicantDtoToJobApplicants()
        {
            // Arrange
            var jobApplicantDto = new JobApplicantDto
            {
                Name = "John Wick",
                Email = "john.wick@example.com",
                Position = "Developer",
                Resume = new FormFile(null, 0, 0, null, "resume.pdf"),
                Certifications = new List<IFormFile> { new FormFile(null, 0, 0, null, "cert1.pdf"), new FormFile(null, 0, 0, null, "cert2.pdf") }
            };

            // Act
            var result = _mapper.Map<JobApplicants>(jobApplicantDto);

            // Assert
            Assert.AreEqual(jobApplicantDto.Name, result.Name);
            Assert.AreEqual(jobApplicantDto.Email, result.Email);
            Assert.AreEqual(jobApplicantDto.Position, result.Position);
            Assert.AreEqual(string.Empty, result.ResumeFileName);
            CollectionAssert.AreEqual(new List<string>(), result.CertificationsFilesNames);
        }

        [Test]
        public void ShouldMapJobApplicantsToJobApplicantViewModel()
        {
            // Arrange
            var jobApplicant = new JobApplicants
            {
                Id = new Guid("1f65ebc8-e773-43a9-b336-282dec4cde25"),
                Name = "John Wick",
                Email = "john.wick@example.com",
                Position = "Developer"
            };

            // Act
            var result = _mapper.Map<JobApplicantViewModel>(jobApplicant);

            // Assert
            Assert.AreEqual(jobApplicant.Id, result.Id);
            Assert.AreEqual(jobApplicant.Name, result.Name);
            Assert.AreEqual(jobApplicant.Email, result.Email);
            Assert.AreEqual(jobApplicant.Position, result.Position);
        }

        [Test]
        public void ShouldMapJobApplicantsToJobApplicantDtoWithEmptyFields()
        {
            // Arrange
            var jobApplicant = new JobApplicants
            {
                Id = new Guid("1f65ebc8-e773-43a9-b336-282dec4cde25"),
                Name = "John Jones",
                Email = "john.jones@example.com",
                Position = "Designer",
                ResumeFileName = "resume.pdf",
                CertificationsFilesNames = new List<string> { "cert1.pdf", "cert2.pdf" }
            };

            // Act
            var result = _mapper.Map<JobApplicantDto>(jobApplicant);

            // Assert
            Assert.AreEqual(jobApplicant.Name, result.Name);
            Assert.AreEqual(jobApplicant.Email, result.Email);
            Assert.AreEqual(jobApplicant.Position, result.Position);
            Assert.IsNull(result.Resume);
            Assert.IsNull(result.Certifications);
        }
    }
}
