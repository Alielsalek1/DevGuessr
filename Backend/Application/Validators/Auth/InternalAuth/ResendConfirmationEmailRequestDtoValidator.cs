using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class ResendConfirmationEmailRequestDtoValidator : AbstractValidator<ResendConfirmationEmailRequestDto>
{
    public ResendConfirmationEmailRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}
