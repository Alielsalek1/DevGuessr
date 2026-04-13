using Application.DTOs.Clusterdle;
using FluentValidation;

namespace Application.Validators.Clusterdle;

public class AddClusterdleWordByGroupNameRequestDtoValidator : AbstractValidator<AddClusterdleWordByGroupNameRequestDto>
{
    public AddClusterdleWordByGroupNameRequestDtoValidator()
    {
        RuleFor(x => x.Word)
            .NotEmpty().WithMessage("Word is required.");
    }
}
