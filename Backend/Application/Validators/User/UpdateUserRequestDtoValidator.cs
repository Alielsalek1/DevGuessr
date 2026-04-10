using Application.DTOs.User;
using FluentValidation;

namespace Application.Validators.User;

public class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequestDto>
{
    public UpdateUserRequestDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Invalid phone number format");

        RuleFor(x => x.Address)
            .MinimumLength(5).When(x => !string.IsNullOrEmpty(x.Address))
            .WithMessage("Address must be at least 5 characters long");
            
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Address) || !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("At least one field (Address or PhoneNumber) must be provided for update.");
    }
}
