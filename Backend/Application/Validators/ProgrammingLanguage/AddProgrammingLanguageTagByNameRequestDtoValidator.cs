using Application.DTOs.ProgrammingLanguage;
using FluentValidation;

namespace Application.Validators.ProgrammingLanguage;

public class AddProgrammingLanguageTagByNameRequestDtoValidator : AbstractValidator<AddProgrammingLanguageTagByNameRequestDto>
{
    public AddProgrammingLanguageTagByNameRequestDtoValidator()
    {
        RuleFor(x => x.Tag)
            .NotEmpty().WithMessage("Tag is required.")
            .MaximumLength(50).WithMessage("Tag must not exceed 50 characters.");
    }
}
