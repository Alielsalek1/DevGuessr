using Application.DTOs.Langdle;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Langdle;

public class CreateLangdleRequestDtoValidator : AbstractValidator<CreateLangdleRequestDto>
{
    public CreateLangdleRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.YearFirstAppeared)
            .GreaterThan(1940).WithMessage("Year must be greater than 1940.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Year must not be in the future.");

        RuleFor(x => x.TypeChecking)
            .NotEmpty().WithMessage("Type checking is required.")
            .Must(v => Enum.TryParse<TypeChecking>(v, true, out _))
            .WithMessage("Type checking must be one of: STATIC, DYNAMIC, GRADUAL.");

        RuleFor(x => x.Memory)
            .NotEmpty().WithMessage("Memory is required.")
            .Must(v => Enum.TryParse<Memory>(v, true, out _))
            .WithMessage("Memory must be one of: GC, MANUAL, OWNERSHIP, REFERENCE_COUNTED.");

        RuleFor(x => x.ScopeSyntax)
            .NotEmpty().WithMessage("Scope syntax is required.")
            .Must(v => Enum.TryParse<ScopeSyntax>(v, true, out _))
            .WithMessage("Scope syntax must be one of: BRACES, INDENTATION, KEYWORDS, MIXED.");

        RuleFor(x => x.Semicolons)
            .NotEmpty().WithMessage("Semicolons is required.")
            .Must(v => Enum.TryParse<Semicolons>(v, true, out _))
            .WithMessage("Semicolons must be one of: REQUIRED, OPTIONAL, NONE.");

        RuleFor(x => x.Tags)
            .NotNull().WithMessage("Tags are required.")
            .Must(t => t.Count > 0).WithMessage("At least one tag is required.");
    }
}
