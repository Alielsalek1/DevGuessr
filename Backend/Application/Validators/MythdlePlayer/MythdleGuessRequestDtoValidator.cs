using Application.DTOs.MythdlePlayer;
using FluentValidation;

namespace Application.Validators.MythdlePlayer;

public class MythdleGuessRequestDtoValidator : AbstractValidator<MythdleGuessRequestDto>
{
    public MythdleGuessRequestDtoValidator()
    {
        RuleFor(x => x.PuzzleId)
            .NotEmpty().WithMessage("PuzzleId is required.");

        RuleFor(x => x.GuessedTargetName)
            .NotEmpty().WithMessage("GuessedTargetName is required.")
            .MaximumLength(100).WithMessage("GuessedTargetName must not exceed 100 characters.");
    }
}
