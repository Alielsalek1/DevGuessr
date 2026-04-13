using Application.DTOs.Clusterdle;
using FluentValidation;

namespace Application.Validators.Clusterdle;

public class CreateClusterdleRequestDtoValidator : AbstractValidator<CreateClusterdleRequestDto>
{
    public CreateClusterdleRequestDtoValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty().WithMessage("GroupName is required.");

        RuleFor(x => x.DifficultyLevel)
            .InclusiveBetween(1, 4).WithMessage("DifficultyLevel must be between 1 and 4.");

        RuleFor(x => x.Words)
            .NotNull().WithMessage("Words list cannot be null.")
            .Must(w => w.Count == 4).WithMessage("Exactly 4 words are required.");

        RuleFor(x => x.SuccessMessage)
            .NotEmpty().WithMessage("SuccessMessage is required.");
    }
}
