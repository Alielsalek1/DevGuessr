using Application.DTOs.Clusterdle;
using FluentValidation;

namespace Application.Validators.Clusterdle;

public class UpdateClusterdleByGroupNameRequestDtoValidator : AbstractValidator<UpdateClusterdleByGroupNameRequestDto>
{
    public UpdateClusterdleByGroupNameRequestDtoValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty().WithMessage("GroupName cannot be empty.").When(x => x.GroupName != null);

        RuleFor(x => x.DifficultyLevel)
            .InclusiveBetween(1, 4).WithMessage("DifficultyLevel must be between 1 and 4.").When(x => x.DifficultyLevel.HasValue);

        RuleFor(x => x.SuccessMessage)
            .NotEmpty().WithMessage("SuccessMessage cannot be empty.").When(x => x.SuccessMessage != null);
    }
}
