using FluentValidation.TestHelper;
using JobApplicationPortal.Models.DTOModels;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace JobApplicationPortal.Tests.Validators
{
    [TestFixture]
    public class JobApplicantDtoValidatorTests
    {
        private JobApplicantDtoValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new JobApplicantDtoValidator();
        }

        [Test]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var model = new JobApplicantDto { Name = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name is required.");
        }

        [Test]
        public void Should_Not_Have_Error_When_Name_Is_Specified()
        {
            var model = new JobApplicantDto { Name = "John Wick" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Test]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var model = new JobApplicantDto { Email = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email is required.");
        }

        [Test]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new JobApplicantDto { Email = "invalidemail" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Please provide a valid email.");
        }

        [Test]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {
            var model = new JobApplicantDto { Email = "john.wick@example.com" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void Should_Have_Error_When_Position_Is_Empty()
        {
            var model = new JobApplicantDto { Position = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Position).WithErrorMessage("Position is required.");
        }

        [Test]
        public void Should_Not_Have_Error_When_Position_Is_Specified()
        {
            var model = new JobApplicantDto { Position = "Assassin" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Position);
        }

        [Test]
        public void Should_Have_Error_When_Resume_Is_Null()
        {
            var model = new JobApplicantDto { Resume = null };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Resume).WithErrorMessage("Resume is required.");
        }

        [Test]
        public void Should_Not_Have_Error_When_Resume_Is_Specified()
        {
            var model = new JobApplicantDto { Resume = Substitute.For<IFormFile>() };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Resume);
        }
    }
}
