using Application.DTOs.LogodleTarget;
using FluentValidation;

namespace Application.Validators.LogodleTarget;

public class CreateLogodleTargetRequestDtoValidator : AbstractValidator<CreateLogodleTargetRequestDto>
{
    public CreateLogodleTargetRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Image)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Image is required.")
            .Must(file => file.Length > 0).WithMessage("Image file cannot be empty.");
    }
}
