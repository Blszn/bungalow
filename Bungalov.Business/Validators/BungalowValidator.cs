using Bungalov.Core.Varliklar;
using FluentValidation;

namespace Bungalov.Business.Validators;

public class BungalowValidator : AbstractValidator<Bungalow>
{
    public BungalowValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Bungalov adı boş olamaz.");
        RuleFor(x => x.PricePerNight).GreaterThan(0).WithMessage("Bungalov fiyatı 0'dan büyük olmalıdır.");
        RuleFor(x => x.Capacity).GreaterThanOrEqualTo(1).WithMessage("Kapasite en az 1 kişi olmalıdır.");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Kategori seçimi zorunludur.");
    }
}
