using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using JobApplicationApi.Controllers;
using JobApplicationPortal.Backend.Responses;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.DTOModels;
using JobApplicationPortal.Models.Interfaces;
using JobApplicationPortal.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using System.IO.Compression;

namespace JobApplicationPortal.Tests.Controllers
{
    public class JobApplicantsControllerTests
    {
        private IJobApplicantsRepository _repository;
        private IBlobService _blobService;
        private IMapper _mapper;
        private IValidator<JobApplicantDto> _validator;
        private JobApplicantsController _controller;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<IJobApplicantsRepository>();
            _blobService = Substitute.For<IBlobService>();
            _mapper = Substitute.For<IMapper>();
            _validator = Substitute.For<IValidator<JobApplicantDto>>();

            _controller = new JobApplicantsController(
                _repository,
                _blobService,
                _mapper,
                _validator
            );
        }

        [Test]
        public async Task Submit_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var model = new JobApplicantDto();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required.")
            });
            _validator.ValidateAsync(model, default).Returns(validationResult);

            // Act
            var result = await _controller.Submit(model);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var responseObject = badRequestResult.Value as JobApplicationResponse;
            Assert.IsFalse(responseObject.Success);
            Assert.AreEqual("Validation errors occurred.", responseObject.Message);
        }

        [Test]
        public async Task Submit_ReturnsOk_WhenValidationPasses()
        {
            // Arrange
            var model = new JobApplicantDto
            {
                Name = "John Wick",
                Email = "john.wick@example.com",
                Position = "Software Manager",
                Resume = Substitute.For<IFormFile>()
            };
            model.Resume.FileName.Returns("resume.pdf");

            var validationResult = new ValidationResult();
            _validator.ValidateAsync(model, default).Returns(validationResult);

            var jobApplicant = new JobApplicants { UniqueId = "someUniqueId" };
            _mapper.Map<JobApplicants>(model).Returns(jobApplicant);

            // Act
            var result = await _controller.Submit(model);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var responseObject = okResult.Value as JobApplicationResponse;
            Assert.IsTrue(responseObject.Success);
            Assert.AreEqual("Application submitted successfully.", responseObject.Message);

            await _repository.Received(1).AddJobApplicantAsync(Arg.Any<JobApplicants>());
            await _repository.Received(1).SaveChangesAsync();
            await _blobService.Received(1).UploadFileBlobAsync(model.Resume, jobApplicant.UniqueId, "Resumes/resume.pdf");
        }

        [Test]
        public async Task DownloadResume_ReturnsNotFound_WhenJobApplicantNotFound()
        {
            // Arrange
            var uniqueId = "someUniqueId";
            _repository.GetJobApplicantByUniqueIdAsync(uniqueId).Returns((JobApplicants)null);

            // Act
            var result = await _controller.DownloadResume(uniqueId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var responseObject = notFoundResult.Value as JobApplicationResponse;
            Assert.IsFalse(responseObject.Success);
            Assert.AreEqual("Job applicant not found.", responseObject.Message);
        }

        [Test]
        public async Task DownloadResume_ReturnsBadRequest_WhenResumeFileNameIsEmpty()
        {
            // Arrange
            var uniqueId = "someUniqueId";
            var jobApplicant = new JobApplicants { UniqueId = uniqueId, ResumeFileName = "" };
            _repository.GetJobApplicantByUniqueIdAsync(uniqueId).Returns(jobApplicant);

            // Act
            var result = await _controller.DownloadResume(uniqueId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var responseObject = badRequestResult.Value as JobApplicationResponse;
            Assert.IsFalse(responseObject.Success);
            Assert.AreEqual("File path is required.", responseObject.Message);
        }

        [Test]
        public async Task DownloadResume_ReturnsNotFound_WhenFileDataIsNull()
        {
            // Arrange
            var uniqueId = "12345";
            var jobApplicant = new JobApplicants { UniqueId = uniqueId, ResumeFileName = "resume.pdf" };
            _repository.GetJobApplicantByUniqueIdAsync(uniqueId).Returns(jobApplicant);

            var relativePath = $"Resumes/{jobApplicant.ResumeFileName}";
            _blobService.DownloadFileBlobAsync(uniqueId, relativePath).Returns((byte[])null);

            // Act
            var result = await _controller.DownloadResume(uniqueId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var responseObject = notFoundResult.Value as JobApplicationResponse;
            Assert.IsFalse(responseObject.Success);
            Assert.AreEqual("File not found.", responseObject.Message);
        }

        [Test]
        public async Task DownloadResume_ReturnsFile_WhenFileDataIsNotNull()
        {
            // Arrange
            var uniqueId = "12345";
            var jobApplicant = new JobApplicants { UniqueId = uniqueId, ResumeFileName = "resume.pdf" };
            _repository.GetJobApplicantByUniqueIdAsync(uniqueId).Returns(jobApplicant);

            var relativePath = $"Resumes/{jobApplicant.ResumeFileName}";
            var fileData = new byte[] { 1, 2, 3 };
            _blobService.DownloadFileBlobAsync(uniqueId, relativePath).Returns(fileData);

            // Act
            var result = await _controller.DownloadResume(uniqueId);

            // Assert
            Assert.IsInstanceOf<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.IsNotNull(fileResult);
            Assert.AreEqual("application/octet-stream", fileResult.ContentType);
            Assert.AreEqual(fileData, fileResult.FileContents);
            Assert.AreEqual("resume.pdf", fileResult.FileDownloadName);
        }

        [Test]
        public async Task DownloadCertifications_ReturnsNotFound_WhenJobApplicantNotFound()
        {
            // Arrange
            var uniqueId = "someGuid";
            _repository.GetJobApplicantByUniqueIdAsync(uniqueId).Returns((JobApplicants)null);

            // Act
            var result = await _controller.DownloadCertifications(uniqueId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var responseObject = notFoundResult.Value as JobApplicationResponse;
            Assert.IsNotNull(responseObject);
            Assert.IsFalse(responseObject.Success);
            Assert.AreEqual("Job applicant not found.", responseObject.Message);
        }

        [Test]
        public async Task DownloadCertifications_ReturnsBadRequest_WhenNoCertificationsFound()
        {
            // Arrange
            var uniqueId = "someGuidId";
            var jobApplicant = new JobApplicants { UniqueId = uniqueId, CertificationsFilesNames = new List<string>() };
            _repository.GetJobApplicantByUniqueIdAsync(uniqueId).Returns(jobApplicant);

            // Act
            var result = await _controller.DownloadCertifications(uniqueId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var responseObject = badRequestResult.Value as JobApplicationResponse;
            Assert.IsNotNull(responseObject);
            Assert.IsFalse(responseObject.Success);
            Assert.AreEqual("No certifications found.", responseObject.Message);
        }

        [Test]
        public async Task DownloadCertifications_ReturnsZipFile_WhenCertificationsExist()
        {
            // Arrange
            var uniqueId = "12345";
            var jobApplicant = new JobApplicants
            {
                UniqueId = uniqueId,
                Name = "John Wick",
                CertificationsFilesNames = new List<string> { "cert1.pdf", "cert2.pdf" }
            };
            _repository.GetJobApplicantByUniqueIdAsync(uniqueId).Returns(jobApplicant);

            var cert1Data = new byte[] { 1, 2, 3 };
            var cert2Data = new byte[] { 4, 5, 6 };
            _blobService.DownloadFileBlobAsync(uniqueId, "Certifications/cert1.pdf").Returns(cert1Data);
            _blobService.DownloadFileBlobAsync(uniqueId, "Certifications/cert2.pdf").Returns(cert2Data);

            // Act
            var result = await _controller.DownloadCertifications(uniqueId);

            // Assert
            Assert.IsInstanceOf<FileStreamResult>(result);
            var fileResult = result as FileStreamResult;
            Assert.IsNotNull(fileResult);
            Assert.AreEqual("application/zip", fileResult.ContentType);
            Assert.AreEqual($"{jobApplicant.Name}_Certifications.zip", fileResult.FileDownloadName);

            using (var memoryStream = new MemoryStream())
            {
                await fileResult.FileStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                {
                    Assert.AreEqual(2, archive.Entries.Count);

                    var entry1 = archive.GetEntry("cert1.pdf");
                    Assert.IsNotNull(entry1);
                    using (var entryStream = entry1.Open())
                    using (var entryMemoryStream = new MemoryStream())
                    {
                        entryStream.CopyTo(entryMemoryStream);
                        Assert.AreEqual(cert1Data, entryMemoryStream.ToArray());
                    }

                    var entry2 = archive.GetEntry("cert2.pdf");
                    Assert.IsNotNull(entry2);
                    using (var entryStream = entry2.Open())
                    using (var entryMemoryStream = new MemoryStream())
                    {
                        entryStream.CopyTo(entryMemoryStream);
                        Assert.AreEqual(cert2Data, entryMemoryStream.ToArray());
                    }
                }
            }
        }


        [Test]
        public async Task GetAllJobApplicants_ReturnsOk_WithListOfApplicants()
        {
            // Arrange
            var jobApplicants = new List<JobApplicants>
            {
                new JobApplicants { UniqueId = "1", Name = "John Jones", Email = "john.jones@example.com", Position = "Manager" },
                new JobApplicants { UniqueId = "2", Name = "Jane Smith", Email = "jane.smith@example.com", Position = "Designer" }
            };
            var jobApplicantViewModels = new List<JobApplicantViewModel>
            {
                new JobApplicantViewModel { UniqueId = "1", Name = "John Jones", Email = "john.jones@example.com", Position = "Manager" },
                new JobApplicantViewModel { UniqueId = "2", Name = "Jane Smith", Email = "jane.smith@example.com", Position = "Designer" }
            };

            _repository.GetAllJobApplicantsAsync().Returns(jobApplicants);
            _mapper.Map<List<JobApplicantViewModel>>(jobApplicants).Returns(jobApplicantViewModels);

            // Act
            var result = await _controller.GetAllJobApplicants();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var responseObject = okResult.Value as List<JobApplicantViewModel>;
            Assert.IsNotNull(responseObject);
            Assert.AreEqual(2, responseObject.Count);
            Assert.AreEqual("John Jones", responseObject[0].Name);
            Assert.AreEqual("Jane Smith", responseObject[1].Name);
        }

        [Test]
        public async Task GetJobApplicantById_ReturnsNotFound_WhenApplicantDoesNotExist()
        {
            // Arrange
            var uniqueId = "someUnieuqId";
            _repository.GetJobApplicantByUniqueIdAsync(uniqueId).Returns((JobApplicants)null);

            // Act
            var result = await _controller.GetJobApplicantById(uniqueId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var responseObject = notFoundResult.Value as JobApplicationResponse;
            Assert.IsFalse(responseObject.Success);
            Assert.AreEqual("Job applicant not found.", responseObject.Message);
        }

        [Test]
        public async Task GetJobApplicantById_ReturnsOk_WithJobApplicant()
        {
            // Arrange
            var uniqueId = "someGuidId";
            var jobApplicant = new JobApplicants { UniqueId = uniqueId, Name = "John Wick", Email = "john.wick@example.com", Position = "Programmer" };
            var jobApplicantViewModel = new JobApplicantViewModel { UniqueId = uniqueId, Name = "John Wick", Email = "john.wick@example.com", Position = "Programmer" };

            _repository.GetJobApplicantByUniqueIdAsync(uniqueId).Returns(jobApplicant);
            _mapper.Map<JobApplicantViewModel>(jobApplicant).Returns(jobApplicantViewModel);

            // Act
            var result = await _controller.GetJobApplicantById(uniqueId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var responseObject = okResult.Value as JobApplicantViewModel;
            Assert.IsNotNull(responseObject);
            Assert.AreEqual("John Wick", responseObject.Name);
            Assert.AreEqual("john.wick@example.com", responseObject.Email);
            Assert.AreEqual("Programmer", responseObject.Position);
        }
    }
}
