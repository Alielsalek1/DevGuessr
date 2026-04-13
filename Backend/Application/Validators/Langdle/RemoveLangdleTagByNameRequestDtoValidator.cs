using Application.DTOs.Langdle;
using FluentValidation;

namespace Application.Validators.Langdle;

public class RemoveLangdleTagByNameRequestDtoValidator : AbstractValidator<RemoveLangdleTagByNameRequestDto>
{
    public RemoveLangdleTagByNameRequestDtoValidator()
    {
        RuleFor(x => x.Tag)
            .NotEmpty().WithMessage("Tag is required.")
            .MaximumLength(50).WithMessage("Tag must not exceed 50 characters.");
    }
}
