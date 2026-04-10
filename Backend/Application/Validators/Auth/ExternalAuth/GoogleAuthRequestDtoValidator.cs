using FluentValidation;
using Application.DTOs.ExternalAuth;

namespace Application.Validators.Auth;

public class GoogleAuthRequestDtoValidator : AbstractValidator<GoogleAuthRequestDto>
{
    public GoogleAuthRequestDtoValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("IdToken is required.");
    }
}