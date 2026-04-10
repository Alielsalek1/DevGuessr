using Application.DTOs.TechdlePlayer;
using FluentValidation;

namespace Application.Validators.TechdlePlayer;

public class TechdleGuessRequestDtoValidator : AbstractValidator<TechdleGuessRequestDto>
{
    public TechdleGuessRequestDtoValidator()
    {
        RuleFor(x => x.PuzzleId)
            .NotEmpty().WithMessage("PuzzleId is required.");

        RuleFor(x => x.GuessedLanguageName)
            .NotEmpty().WithMessage("GuessedLanguageName is required.");
    }
}
