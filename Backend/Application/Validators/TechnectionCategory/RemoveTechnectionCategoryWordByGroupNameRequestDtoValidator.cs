using Application.DTOs.TechnectionCategory;
using FluentValidation;

namespace Application.Validators.TechnectionCategory;

public class RemoveTechnectionCategoryWordByGroupNameRequestDtoValidator : AbstractValidator<RemoveTechnectionCategoryWordByGroupNameRequestDto>
{
    public RemoveTechnectionCategoryWordByGroupNameRequestDtoValidator()
    {
        RuleFor(x => x.Word)
            .NotEmpty().WithMessage("Word is required.");
    }
}
