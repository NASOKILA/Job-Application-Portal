using AutoMapper;
using JobApplicationPortal.Backend.API.Mappers;
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

namespace JobApplicationPortal.Tests.Services.IntegrationTests
{
    [TestFixture]
    public class JobApplicantServiceIntegrationTests
    {
        private JobApplicationPortalDbContext _context;
        private JobApplicantService _service;
        private IBlobService _blobService;
        private IMapper _mapper;
        private ILogger<JobApplicantService> _logger;


        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<JobApplicationPortalDbContext>()
                .UseSqlServer("Server=(local);Database=JobApplicationPortalTestDB;Trusted_Connection=True;Integrated Security=true;TrustServerCertificate=True")
                .Options;

            _context = new JobApplicationPortalDbContext(options);
            _blobService = Substitute.For<IBlobService>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApplicantMappingProfile>()).CreateMapper();
            _logger = Substitute.For<ILogger<JobApplicantService>>();

            _service = new JobApplicantService(_context, _blobService, _mapper, _logger);

            _context.Database.EnsureCreated();
            SeedDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedDatabase()
        {
            var jobApplicants = new[]
            {
                new JobApplicants { Id = new Guid("c96aaa2b-faeb-488e-ad3c-67739017fd22"), Name = "John Jones", Email = "john.jones@example.com", Position = "Developer", CertificationsFilesNames = new List<string>(), ResumeFileName = "MyResume1.pdf" },
                new JobApplicants { Id = new Guid("4593bd86-6762-41ee-97a0-2418c56d04ee"), Name = "Jane Smith", Email = "jane.smith@example.com", Position = "Designer", CertificationsFilesNames = new List<string>(), ResumeFileName = "MyResume2.pdf" }
            };

            _context.JobApplicants.AddRange(jobApplicants);
            _context.SaveChanges();
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
                Resume = Substitute.For<IFormFile>()
            };

            // Act
            var result = await _service.AddJobApplicantAsync(model);

            // Assert
            var addedApplicant = await _context.JobApplicants.FindAsync(result.Id);
            Assert.IsNotNull(addedApplicant);
            Assert.AreEqual("Jack Sparrow", addedApplicant.Name);
            Assert.AreEqual("jack.sparrow@example.com", addedApplicant.Email);
            Assert.AreEqual("Manager", addedApplicant.Position);
        }

        [Test]
        public async Task GetJobApplicantByIdAsync_ShouldReturnJobApplicant()
        {
            // Arrange
            var id = new Guid("c96aaa2b-faeb-488e-ad3c-67739017fd22");

            // Act
            var result = await _service.GetJobApplicantByIdAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John Jones", result.Name);
        }

        [Test]
        public async Task GetAllJobApplicantsAsync_ShouldReturnAllJobApplicants()
        {
            // Act
            var result = await _service.GetAllJobApplicantsAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(a => a.Name == "John Jones"));
            Assert.IsTrue(result.Any(a => a.Name == "Jane Smith"));
        }
    }
}
