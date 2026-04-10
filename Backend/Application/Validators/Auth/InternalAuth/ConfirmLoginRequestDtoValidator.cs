using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class ConfirmLoginRequestDtoValidator : AbstractValidator<ConfirmLoginRequestDto>
{
    public ConfirmLoginRequestDtoValidator()
    {
        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("Confirmation token is required.");
    }
}
