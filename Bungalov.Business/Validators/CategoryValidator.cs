using Bungalov.Core.Varliklar;
using FluentValidation;

namespace Bungalov.Business.Validators;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Kategori adı boş olamaz.");
    }
}
