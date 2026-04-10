using Application.DTOs.LogodlePlayer;
using FluentValidation;

namespace Application.Validators.LogodlePlayer;

public class LogodleGuessRequestDtoValidator : AbstractValidator<LogodleGuessRequestDto>
{
    public LogodleGuessRequestDtoValidator()
    {
        RuleFor(x => x.PuzzleId)
            .NotEmpty().WithMessage("PuzzleId is required.");

        RuleFor(x => x.GuessedTargetName)
            .NotEmpty().WithMessage("GuessedTargetName is required.")
            .MaximumLength(100).WithMessage("GuessedTargetName must not exceed 100 characters.");

        RuleFor(x => x.AttemptNumber)
            .InclusiveBetween(1, 6).WithMessage("AttemptNumber must be between 1 and 6.");
    }
}