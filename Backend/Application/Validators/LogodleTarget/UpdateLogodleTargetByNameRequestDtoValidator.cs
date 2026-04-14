using Application.DTOs.LogodleTarget;
using FluentValidation;

namespace Application.Validators.LogodleTarget;

public class UpdateLogodleTargetByNameRequestDtoValidator : AbstractValidator<UpdateLogodleTargetByNameRequestDto>
{
    public UpdateLogodleTargetByNameRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.ImagePath)
            .MaximumLength(500).WithMessage("Image path must not exceed 500 characters.")
            .When(x => x.ImagePath is not null);

        RuleFor(x => x.BlurredImageUrls)
            .Must(urls => urls!.Count == 6).WithMessage("Exactly 6 blurred image URLs are required.")
            .When(x => x.BlurredImageUrls is not null);

        RuleForEach(x => x.BlurredImageUrls)
            .NotEmpty().WithMessage("Blurred image URL cannot be empty.")
            .MaximumLength(500).WithMessage("Blurred image URL must not exceed 500 characters.")
            .When(x => x.BlurredImageUrls is not null);
    }
}
