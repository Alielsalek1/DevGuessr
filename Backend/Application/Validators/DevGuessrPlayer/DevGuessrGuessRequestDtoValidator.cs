using Application.DTOs.DevGuessrPlayer;
using FluentValidation;

namespace Application.Validators.DevGuessrPlayer;

public class DevGuessrGuessRequestDtoValidator : AbstractValidator<DevGuessrGuessRequestDto>
{
    public DevGuessrGuessRequestDtoValidator()
    {
        RuleFor(x => x.PuzzleId)
            .NotEmpty().WithMessage("PuzzleId is required.");

        RuleFor(x => x.GuessedLanguageName)
            .NotEmpty().WithMessage("GuessedLanguageName is required.");
    }
}
