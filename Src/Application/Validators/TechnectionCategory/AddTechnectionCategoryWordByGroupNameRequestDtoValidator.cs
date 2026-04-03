using Application.DTOs.TechnectionCategory;
using FluentValidation;

namespace Application.Validators.TechnectionCategory;

public class AddTechnectionCategoryWordByGroupNameRequestDtoValidator : AbstractValidator<AddTechnectionCategoryWordByGroupNameRequestDto>
{
    public AddTechnectionCategoryWordByGroupNameRequestDtoValidator()
    {
        RuleFor(x => x.Word)
            .NotEmpty().WithMessage("Word is required.");
    }
}
