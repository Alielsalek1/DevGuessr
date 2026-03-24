using Application.DTOs.ProgrammingLanguage;
using FluentValidation;

namespace Application.Validators.ProgrammingLanguage;

public class RemoveProgrammingLanguageTagByNameRequestDtoValidator : AbstractValidator<RemoveProgrammingLanguageTagByNameRequestDto>
{
    public RemoveProgrammingLanguageTagByNameRequestDtoValidator()
    {
        RuleFor(x => x.Tag)
            .NotEmpty().WithMessage("Tag is required.")
            .MaximumLength(50).WithMessage("Tag must not exceed 50 characters.");
    }
}
