using Application.DTOs.Langdle;
using FluentValidation;

namespace Application.Validators.Langdle;

public class AddLangdleTagByNameRequestDtoValidator : AbstractValidator<AddLangdleTagByNameRequestDto>
{
    public AddLangdleTagByNameRequestDtoValidator()
    {
        RuleFor(x => x.Tag)
            .NotEmpty().WithMessage("Tag is required.")
            .MaximumLength(50).WithMessage("Tag must not exceed 50 characters.");
    }
}
