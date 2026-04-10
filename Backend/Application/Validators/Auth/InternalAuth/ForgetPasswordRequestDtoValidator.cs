using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class ForgetPasswordRequestDtoValidator : AbstractValidator<ForgetPasswordRequestDto>
{
    public ForgetPasswordRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
