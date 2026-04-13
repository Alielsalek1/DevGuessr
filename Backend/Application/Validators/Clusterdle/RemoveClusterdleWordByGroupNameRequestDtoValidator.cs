using Application.DTOs.Clusterdle;
using FluentValidation;

namespace Application.Validators.Clusterdle;

public class RemoveClusterdleWordByGroupNameRequestDtoValidator : AbstractValidator<RemoveClusterdleWordByGroupNameRequestDto>
{
    public RemoveClusterdleWordByGroupNameRequestDtoValidator()
    {
        RuleFor(x => x.Word)
            .NotEmpty().WithMessage("Word is required.");
    }
}
