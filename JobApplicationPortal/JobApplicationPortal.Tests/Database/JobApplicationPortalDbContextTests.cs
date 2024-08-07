using JobApplicationPortal.DB;
using JobApplicationPortal.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace JobApplicationPortal.Tests.Database
{
    public class JobApplicationPortalDbContextTests
    {
        private DbContextOptions<JobApplicationPortalDbContext> _options;
        private JobApplicationPortalDbContext _context;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<JobApplicationPortalDbContext>()
                .UseInMemoryDatabase(databaseName: "JobApplicationPortalTestDb")
                .Options;

            _context = new JobApplicationPortalDbContext(_options);

            SeedDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the database after each test
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
        public async Task CanAddJobApplicant()
        {
            // Arrange
            var jobApplicant = new JobApplicants { Id = new Guid("4593bd86-6762-41ee-97a0-2418c56d04ee"), Name = "Jack Sparrow", Email = "jack.sparrow@example.com", Position = "Manager", CertificationsFilesNames = new List<string>(), ResumeFileName = "MyResume3.pdf" };

            // Act
            _context.JobApplicants.Add(jobApplicant);
            await _context.SaveChangesAsync();

            // Assert
            var addedApplicant = await _context.JobApplicants.FindAsync(3);
            Assert.IsNotNull(addedApplicant);
            Assert.AreEqual("Jack Sparrow", addedApplicant.Name);
            Assert.AreEqual("jack.sparrow@example.com", addedApplicant.Email);
            Assert.AreEqual("Manager", addedApplicant.Position);
        }

        [Test]
        public async Task CanUpdateJobApplicant()
        {
            // Arrange
            var jobApplicant = await _context.JobApplicants.FindAsync(1);
            jobApplicant.Name = "Johnathan Wick";

            // Act
            _context.JobApplicants.Update(jobApplicant);
            await _context.SaveChangesAsync();

            // Assert
            var updatedApplicant = await _context.JobApplicants.FindAsync(1);
            Assert.IsNotNull(updatedApplicant);
            Assert.AreEqual("Johnathan Wick", updatedApplicant.Name);
        }

        [Test]
        public async Task CanDeleteJobApplicant()
        {
            // Arrange
            var jobApplicant = await _context.JobApplicants.FindAsync(2);

            // Act
            _context.JobApplicants.Remove(jobApplicant);
            await _context.SaveChangesAsync();

            // Assert
            var deletedApplicant = await _context.JobApplicants.FindAsync(2);
            Assert.IsNull(deletedApplicant);
        }

        [Test]
        public async Task CanRetrieveJobApplicants()
        {
            // Act
            var jobApplicants = await _context.JobApplicants.ToListAsync();

            // Assert
            Assert.AreEqual(2, jobApplicants.Count);
            Assert.IsTrue(jobApplicants.Any(a => a.Name == "John Jones"));
            Assert.IsTrue(jobApplicants.Any(a => a.Name == "Jane Smith"));
        }
    }
}
