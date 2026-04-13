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

        RuleFor(x => x.TypingDiscipline)
            .NotEmpty().WithMessage("Typing discipline is required.")
            .Must(v => Enum.TryParse<TypingDiscipline>(v, true, out _))
            .WithMessage("Typing discipline must be one of: Static, Dynamic.");

        RuleFor(x => x.TypeStrength)
            .NotEmpty().WithMessage("Type strength is required.")
            .Must(v => Enum.TryParse<TypeStrength>(v, true, out _))
            .WithMessage("Type strength must be one of: Strong, Weak.");

        RuleFor(x => x.ExecutionModel)
            .NotEmpty().WithMessage("Execution model is required.")
            .Must(v => Enum.TryParse<ExecutionModel>(v, true, out _))
            .WithMessage("Execution model must be one of: Compiled, Interpreted, BytecodeJIT.");

        RuleFor(x => x.MemoryManagement)
            .NotEmpty().WithMessage("Memory management is required.")
            .Must(v => Enum.TryParse<MemoryManagement>(v, true, out _))
            .WithMessage("Memory management must be one of: GarbageCollected, Manual, OwnershipBorrowing, ARC.");

        RuleFor(x => x.Tags)
            .NotNull().WithMessage("Tags are required.")
            .Must(t => t.Count > 0).WithMessage("At least one tag is required.");
    }
}
