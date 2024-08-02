using JobApplicationPortal.DB;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace JobApplicationPortal.Tests.Repositories
{
    public class JobApplicantsRepositoryTests
    {
        private DbContextOptions<JobApplicationPortalDbContext> _options;
        private JobApplicationPortalDbContext _context;
        private JobApplicantsRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<JobApplicationPortalDbContext>()
                .UseInMemoryDatabase(databaseName: "JobApplicationPortalTestDb")
                .Options;

            _context = new JobApplicationPortalDbContext(_options);
            _repository = new JobApplicantsRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddJobApplicantAsync_AddsJobApplicant()
        {
            // Arrange
            var jobApplicant = new JobApplicants
            {
                UniqueId = "1",
                Name = "John Wick",
                Email = "john.wick@example.com",
                Position = "Developer",
                ResumeFileName = "myResumeFile1.png",
                CertificationsFilesNames = new List<string>() { }
            };

            // Act
            await _repository.AddJobApplicantAsync(jobApplicant);
            await _repository.SaveChangesAsync();

            // Assert
            var addedApplicant = await _context.JobApplicants.FindAsync(1);
            Assert.IsNotNull(addedApplicant);
            Assert.AreEqual("John Wick", addedApplicant.Name);
            Assert.AreEqual("john.wick@example.com", addedApplicant.Email);
            Assert.AreEqual("Developer", addedApplicant.Position);
        }

        [Test]
        public async Task GetJobApplicantByUniqueIdAsync_ReturnsJobApplicant()
        {
            // Arrange
            var jobApplicant = new JobApplicants
            {
                UniqueId = "1",
                Name = "John Wick",
                Email = "john.wick@example.com",
                Position = "Developer",
                ResumeFileName = "johnWickResume.pdf",
                CertificationsFilesNames = new List<string>() { }
            };
            _context.JobApplicants.Add(jobApplicant);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetJobApplicantByUniqueIdAsync("1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John Wick", result.Name);
            Assert.AreEqual("john.wick@example.com", result.Email);
            Assert.AreEqual("Developer", result.Position);
        }

        [Test]
        public async Task GetAllJobApplicantsAsync_ReturnsAllJobApplicants()
        {
            // Arrange
            var jobApplicants = new List<JobApplicants>
            {
                new JobApplicants { UniqueId = "1", Name = "John Wick", Email = "john.wick@example.com", Position = "Developer", ResumeFileName = "johnWickResume.pdf", CertificationsFilesNames = new List<string>(){ } },
                new JobApplicants { UniqueId = "2", Name = "John Jones", Email = "john.jones@example.com", Position = "Designer", ResumeFileName = "johnJonesResume.pdf", CertificationsFilesNames = new List<string>(){ } }
            };
            _context.JobApplicants.AddRange(jobApplicants);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllJobApplicantsAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Exists(a => a.Name == "John Wick"));
            Assert.IsTrue(result.Exists(a => a.Name == "John Jones"));
        }

        [Test]
        public async Task SaveChangesAsync_SavesChanges()
        {
            // Arrange
            var jobApplicant = new JobApplicants
            {
                UniqueId = "1",
                Name = "John Wick",
                Email = "john.wick@example.com",
                Position = "Developer",
                ResumeFileName = "johnWickResume.pdf",
                CertificationsFilesNames = new List<string>() { }
            };
            _context.JobApplicants.Add(jobApplicant);

            // Act
            await _repository.SaveChangesAsync();

            // Assert
            var addedApplicant = await _context.JobApplicants.FindAsync(1);
            Assert.IsNotNull(addedApplicant);
            Assert.AreEqual("John Wick", addedApplicant.Name);
        }
    }
}
