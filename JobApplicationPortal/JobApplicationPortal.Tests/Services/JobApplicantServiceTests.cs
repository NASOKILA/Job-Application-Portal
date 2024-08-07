using AutoMapper;
using JobApplicationPortal.Backend.Services;
using JobApplicationPortal.DB;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.DTOModels;
using JobApplicationPortal.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace JobApplicationPortal.Tests.Services
{
    [TestFixture]
    public class JobApplicantServiceTests
    {
        private JobApplicationPortalDbContext _context;
        private IBlobService _blobService;
        private IMapper _mapper;
        private ILogger<JobApplicantService> _logger;
        private JobApplicantService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<JobApplicationPortalDbContext>()
                .UseInMemoryDatabase(databaseName: "JobApplicationPortalTestDb")
                .Options;
            _context = new JobApplicationPortalDbContext(options);
            _blobService = Substitute.For<IBlobService>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<JobApplicantService>>();

            _service = new JobApplicantService(_context, _blobService, _mapper, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddJobApplicantAsync_ShouldAddJobApplicant()
        {
            // Arrange
            var model = new JobApplicantDto
            {
                Name = "Jack Sparrow",
                Email = "jack.sparrow@example.com",
                Position = "Manager",
                Resume = Substitute.For<IFormFile>(),
                Certifications = new List<IFormFile> { Substitute.For<IFormFile>() }
            };

            var jobApplicant = new JobApplicants
            {
                Name = model.Name,
                Email = model.Email,
                Position = model.Position,
                ResumeFileName = model.Resume.FileName,
                CertificationsFilesNames = new List<string> { model.Certifications[0].FileName }
            };

            _mapper.Map<JobApplicants>(model).Returns(jobApplicant);

            // Act
            var result = await _service.AddJobApplicantAsync(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Jack Sparrow", result.Name);
            Assert.AreEqual("jack.sparrow@example.com", result.Email);
            Assert.AreEqual("Manager", result.Position);
            await _blobService.Received(1).UploadFileBlobAsync(model.Resume, result.Id.ToString(), $"Resumes/{model.Resume.FileName}");
            await _blobService.Received(1).UploadFileBlobAsync(model.Certifications[0], result.Id.ToString(), $"Certifications/{model.Certifications[0].FileName}");
        }

        [Test]
        public async Task GetJobApplicantByIdAsync_ShouldReturnJobApplicant()
        {
            // Arrange
            var id = Guid.NewGuid();
            var jobApplicant = new JobApplicants { Id = id, Name = "John Jones", Email = "john.jones@example.com", Position = "Developer", CertificationsFilesNames = new List<string>(), ResumeFileName = "resume.pdf" };
            await _context.JobApplicants.AddAsync(jobApplicant);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetJobApplicantByIdAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John Jones", result.Name);
        }

        [Test]
        public async Task GetAllJobApplicantsAsync_ShouldReturnAllJobApplicants()
        {
            // Arrange
            var jobApplicants = new[]
            {
                new JobApplicants { Id = Guid.NewGuid(), Name = "John Jones", Email = "john.jones@example.com", Position = "Developer", CertificationsFilesNames = new List<string>(), ResumeFileName = "resume.pdf" },
                new JobApplicants { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane.smith@example.com", Position = "Designer", CertificationsFilesNames = new List<string>(), ResumeFileName = "resume.pdf" }
            };

            await _context.JobApplicants.AddRangeAsync(jobApplicants);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllJobApplicantsAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Exists(a => a.Name == "John Jones"));
            Assert.IsTrue(result.Exists(a => a.Name == "Jane Smith"));
        }
    }
}
