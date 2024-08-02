using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using JobApplicationApi.Controllers;
using JobApplicationPortal.Backend.Responses;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.DTOModels;
using JobApplicationPortal.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

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
                Name = "John Doe",
                Email = "john.doe@example.com",
                Position = "Developer",
                Resume = Substitute.For<IFormFile>()
            };
            model.Resume.FileName.Returns("resume.pdf");

            var validationResult = new ValidationResult();
            _validator.ValidateAsync(model, default).Returns(validationResult);

            var jobApplicant = new JobApplicants { UniqueId = "12345" };
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
    }
}
