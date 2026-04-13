using Application.DTOs.LangdlePlayer;
using FluentValidation;

namespace Application.Validators.LangdlePlayer;

public class LangdleGuessRequestDtoValidator : AbstractValidator<LangdleGuessRequestDto>
{
    public LangdleGuessRequestDtoValidator()
    {
        RuleFor(x => x.PuzzleId)
            .NotEmpty().WithMessage("PuzzleId is required.");

        RuleFor(x => x.GuessedLanguageName)
            .NotEmpty().WithMessage("GuessedLanguageName is required.");
    }
}
