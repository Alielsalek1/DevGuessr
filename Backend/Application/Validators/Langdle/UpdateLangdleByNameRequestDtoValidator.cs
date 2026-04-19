using Application.DTOs.Langdle;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Langdle;

public class UpdateLangdleByNameRequestDtoValidator : AbstractValidator<UpdateLangdleByNameRequestDto>
{
    public UpdateLangdleByNameRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.YearFirstAppeared)
            .GreaterThan(1940).WithMessage("Year must be greater than 1940.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Year must not be in the future.")
            .When(x => x.YearFirstAppeared is not null);

        RuleFor(x => x.TypeChecking)
            .Must(v => Enum.TryParse<TypeChecking>(v, true, out _))
            .WithMessage("Type checking must be one of: STATIC, DYNAMIC, GRADUAL.")
            .When(x => x.TypeChecking is not null);

        RuleFor(x => x.Memory)
            .Must(v => Enum.TryParse<Memory>(v, true, out _))
            .WithMessage("Memory must be one of: GC, MANUAL, OWNERSHIP, REFERENCE_COUNTED.")
            .When(x => x.Memory is not null);

        RuleFor(x => x.ScopeSyntax)
            .Must(v => Enum.TryParse<ScopeSyntax>(v, true, out _))
            .WithMessage("Scope syntax must be one of: BRACES, INDENTATION, KEYWORDS, MIXED.")
            .When(x => x.ScopeSyntax is not null);

        RuleFor(x => x.Semicolons)
            .Must(v => Enum.TryParse<Semicolons>(v, true, out _))
            .WithMessage("Semicolons must be one of: REQUIRED, OPTIONAL, NONE.")
            .When(x => x.Semicolons is not null);
    }
}
