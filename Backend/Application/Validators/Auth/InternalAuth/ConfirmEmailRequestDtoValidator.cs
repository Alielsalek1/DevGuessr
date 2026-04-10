using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class ConfirmEmailRequestDtoValidator : AbstractValidator<ConfirmEmailRequestDto>
{
    public ConfirmEmailRequestDtoValidator()
    {
        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("Token is required");
    }
}
