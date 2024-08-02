using FluentValidation;
using JobApplicationPortal.Models.DTOModels;

public class JobApplicantDtoValidator : AbstractValidator<JobApplicantDto>
{
    public JobApplicantDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Please provide a valid email.");

        RuleFor(x => x.Position).NotEmpty().WithMessage("Position is required.");

        RuleFor(x => x.Resume).NotNull().WithMessage("Resume is required.");
    }
}
