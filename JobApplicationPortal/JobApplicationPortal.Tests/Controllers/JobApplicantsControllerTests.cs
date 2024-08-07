using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using JobApplicationApi.Controllers;
using JobApplicationPortal.Backend.Responses;
using JobApplicationPortal.Backend.Services;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.DTOModels;
using JobApplicationPortal.Models.Interfaces;
using JobApplicationPortal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using System.Text;

namespace JobApplicationPortal.Tests.Controllers
{
    [TestFixture]
    public class JobApplicantsControllerTests
    {
        private JobApplicantsController _controller;
        private JobApplicantService _jobApplicantService;
        private IBlobService _blobService;
        private IMapper _mapper;
        private IValidator<JobApplicantDto> _validator;

        [SetUp]
        public void SetUp()
        {
            _jobApplicantService = Substitute.For<JobApplicantService>(null, null, null, null);
            _blobService = Substitute.For<IBlobService>();
            _mapper = Substitute.For<IMapper>();
            _validator = Substitute.For<IValidator<JobApplicantDto>>();

            _controller = new JobApplicantsController(_jobApplicantService, _blobService, _mapper, _validator);
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
            _validator.ValidateAsync(model).Returns(Task.FromResult(validationResult));

            // Act
            var result = await _controller.Submit(model);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);

            var response = badRequestResult.Value as JobApplicationResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            Assert.AreEqual("Validation errors occurred.", response.Message);
        }

        //[Test]
        //public async Task Submit_ReturnsOk_WhenValidationSucceeds()
        //{
        //    // Arrange
        //    var model = new JobApplicantDto();
        //    var validationResult = new ValidationResult();
        //    _validator.ValidateAsync(model).Returns(Task.FromResult(validationResult));
        //    _jobApplicantService.AddJobApplicantAsync(model).Returns(new JobApplicants());

        //    // Act
        //    var result = await _controller.Submit(model);

        //    // Assert
        //    var okResult = result as OkObjectResult;
        //    Assert.IsNotNull(okResult);

        //    var response = okResult.Value as JobApplicationResponse;
        //    Assert.IsNotNull(response);
        //    Assert.IsTrue(response.Success);
        //    Assert.AreEqual("Application submitted successfully.", response.Message);
        //}

        //[Test]
        //public async Task DownloadResume_ReturnsNotFound_WhenJobApplicantNotFound()
        //{
        //    // Arrange
        //    var id = Guid.NewGuid();
        //    _jobApplicantService.GetJobApplicantByIdAsync(id).Returns((JobApplicants)null);

        //    // Act
        //    var result = await _controller.DownloadResume(id);

        //    // Assert
        //    var notFoundResult = result as NotFoundObjectResult;
        //    Assert.IsNotNull(notFoundResult);

        //    var response = notFoundResult.Value as JobApplicationResponse;
        //    Assert.IsNotNull(response);
        //    Assert.IsFalse(response.Success);
        //    Assert.AreEqual("Job applicant not found.", response.Message);
        //}

        //[Test]
        //public async Task DownloadResume_ReturnsNotFound_WhenResumeFileNameIsEmpty()
        //{
        //    // Arrange
        //    var id = Guid.NewGuid();
        //    var jobApplicant = new JobApplicants { ResumeFileName = string.Empty };
        //    _jobApplicantService.GetJobApplicantByIdAsync(id).Returns(jobApplicant);

        //    // Act
        //    var result = await _controller.DownloadResume(id);

        //    // Assert
        //    var notFoundResult = result as NotFoundObjectResult;
        //    Assert.IsNotNull(notFoundResult);

        //    var response = notFoundResult.Value as JobApplicationResponse;
        //    Assert.IsNotNull(response);
        //    Assert.IsFalse(response.Success);
        //    Assert.AreEqual("Job applicant not found.", response.Message);
        //}

        //[Test]
        //public async Task DownloadResume_ReturnsFile_WhenResumeExists()
        //{
        //    // Arrange
        //    var id = Guid.NewGuid();
        //    var jobApplicant = new JobApplicants { ResumeFileName = "resume.pdf" };
        //    _jobApplicantService.GetJobApplicantByIdAsync(id).Returns(jobApplicant);
        //    var fileData = Encoding.UTF8.GetBytes("File content");
        //    _blobService.DownloadFileBlobAsync(id.ToString(), "Resumes/resume.pdf").Returns(fileData);

        //    // Act
        //    var result = await _controller.DownloadResume(id);

        //    // Assert
        //    var fileResult = result as FileContentResult;
        //    Assert.IsNotNull(fileResult);
        //    Assert.AreEqual("application/octet-stream", fileResult.ContentType);
        //    Assert.AreEqual("resume.pdf", fileResult.FileDownloadName);
        //    CollectionAssert.AreEqual(fileData, fileResult.FileContents);
        //}

        //[Test]
        //public async Task DownloadCertifications_ReturnsNotFound_WhenJobApplicantNotFound()
        //{
        //    // Arrange
        //    var id = Guid.NewGuid();
        //    _jobApplicantService.GetJobApplicantByIdAsync(id).Returns((JobApplicants)null);

        //    // Act
        //    var result = await _controller.DownloadCertifications(id);

        //    // Assert
        //    var notFoundResult = result as NotFoundObjectResult;
        //    Assert.IsNotNull(notFoundResult);

        //    var response = notFoundResult.Value as JobApplicationResponse;
        //    Assert.IsNotNull(response);
        //    Assert.IsFalse(response.Success);
        //    Assert.AreEqual("Job applicant or certifications not found.", response.Message);
        //}

        //[Test]
        //public async Task DownloadCertifications_ReturnsNotFound_WhenNoCertifications()
        //{
        //    // Arrange
        //    var id = Guid.NewGuid();
        //    var jobApplicant = new JobApplicants { CertificationsFilesNames = new List<string>() };
        //    _jobApplicantService.GetJobApplicantByIdAsync(id).Returns(jobApplicant);

        //    // Act
        //    var result = await _controller.DownloadCertifications(id);

        //    // Assert
        //    var notFoundResult = result as NotFoundObjectResult;
        //    Assert.IsNotNull(notFoundResult);

        //    var response = notFoundResult.Value as JobApplicationResponse;
        //    Assert.IsNotNull(response);
        //    Assert.IsFalse(response.Success);
        //    Assert.AreEqual("Job applicant or certifications not found.", response.Message);
        //}

        //[Test]
        //public async Task DownloadCertifications_ReturnsZipFile_WhenCertificationsExist()
        //{
        //    // Arrange
        //    var id = Guid.NewGuid();
        //    var jobApplicant = new JobApplicants
        //    {
        //        Name = "John Doe",
        //        CertificationsFilesNames = new List<string> { "cert1.pdf", "cert2.pdf" }
        //    };
        //    _jobApplicantService.GetJobApplicantByIdAsync(id).Returns(jobApplicant);

        //    var cert1Data = Encoding.UTF8.GetBytes("Certificate 1");
        //    var cert2Data = Encoding.UTF8.GetBytes("Certificate 2");
        //    _blobService.DownloadFileBlobAsync(id.ToString(), "Certifications/cert1.pdf").Returns(cert1Data);
        //    _blobService.DownloadFileBlobAsync(id.ToString(), "Certifications/cert2.pdf").Returns(cert2Data);

        //    // Act
        //    var result = await _controller.DownloadCertifications(id);

        //    // Assert
        //    var fileResult = result as FileStreamResult;
        //    Assert.IsNotNull(fileResult);
        //    Assert.AreEqual("application/zip", fileResult.ContentType);
        //    Assert.AreEqual($"{jobApplicant.Name}_Certifications.zip", fileResult.FileDownloadName);
        //}

        //[Test]
        //public async Task GetAllJobApplicants_ReturnsOk()
        //{
        //    // Arrange
        //    var jobApplicants = new List<JobApplicants>
        //    {
        //        new JobApplicants { Name = "John Jones", Email = "john.jones@example.com", Position = "Developer" },
        //        new JobApplicants { Name = "Jane Smith", Email = "jane.smith@example.com", Position = "Designer" }
        //    };
        //    _jobApplicantService.GetAllJobApplicantsAsync().Returns(jobApplicants);

        //    var viewModels = new List<JobApplicantViewModel>
        //    {
        //        new JobApplicantViewModel { Name = "John Jones", Email = "john.jones@example.com", Position = "Developer" },
        //        new JobApplicantViewModel { Name = "Jane Smith", Email = "jane.smith@example.com", Position = "Designer" }
        //    };
        //    _mapper.Map<List<JobApplicantViewModel>>(jobApplicants).Returns(viewModels);

        //    // Act
        //    var result = await _controller.GetAllJobApplicants();

        //    // Assert
        //    var okResult = result as OkObjectResult;
        //    Assert.IsNotNull(okResult);

        //    var response = okResult.Value as List<JobApplicantViewModel>;
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual(2, response.Count);
        //    Assert.AreEqual("John Jones", response[0].Name);
        //    Assert.AreEqual("Jane Smith", response[1].Name);
        //}
    }
}
