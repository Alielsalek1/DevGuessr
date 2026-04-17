using Application.DTOs.MythdleTarget;
using FluentValidation;

namespace Application.Validators.MythdleTarget;

public class CreateMythdleTargetDtoValidator : AbstractValidator<CreateMythdleTargetDto>
{
    public CreateMythdleTargetDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(50).WithMessage("Category must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
