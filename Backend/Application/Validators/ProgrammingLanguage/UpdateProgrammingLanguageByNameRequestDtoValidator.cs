using Application.DTOs.ProgrammingLanguage;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.ProgrammingLanguage;

public class UpdateProgrammingLanguageByNameRequestDtoValidator : AbstractValidator<UpdateProgrammingLanguageByNameRequestDto>
{
    public UpdateProgrammingLanguageByNameRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.YearFirstAppeared)
            .GreaterThan(1940).WithMessage("Year must be greater than 1940.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Year must not be in the future.")
            .When(x => x.YearFirstAppeared is not null);

        RuleFor(x => x.TypingDiscipline)
            .Must(v => Enum.TryParse<TypingDiscipline>(v, true, out _))
            .WithMessage("Typing discipline must be one of: Static, Dynamic.")
            .When(x => x.TypingDiscipline is not null);

        RuleFor(x => x.TypeStrength)
            .Must(v => Enum.TryParse<TypeStrength>(v, true, out _))
            .WithMessage("Type strength must be one of: Strong, Weak.")
            .When(x => x.TypeStrength is not null);

        RuleFor(x => x.ExecutionModel)
            .Must(v => Enum.TryParse<ExecutionModel>(v, true, out _))
            .WithMessage("Execution model must be one of: Compiled, Interpreted, BytecodeJIT.")
            .When(x => x.ExecutionModel is not null);

        RuleFor(x => x.MemoryManagement)
            .Must(v => Enum.TryParse<MemoryManagement>(v, true, out _))
            .WithMessage("Memory management must be one of: GarbageCollected, Manual, OwnershipBorrowing, ARC.")
            .When(x => x.MemoryManagement is not null);
    }
}
