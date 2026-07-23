using PAS.Asset.Domain.Funds.Exceptions;
using PAS.Common.DDD;

namespace PAS.Asset.Domain.Funds;

public record FundNav : ValueObject, ISoftDeletable {

    public decimal Value { get; }
    public DateTime Date { get; }
    public DateTime? DeletedAtUtc { get; private set; }

    private FundNav(decimal value, DateTime date) {
        Value = value;
        Date = date;
        DeletedAtUtc = null;
    }

    public static FundNav Create(decimal value, DateTime date) {
        if (value <= 0) throw new InvalidNavValueException(value);
        if (date == default) throw new ArgumentException("La date du NAV ne peut pas être vide.", nameof(date));

        return new FundNav(value, date);
    }

    internal void SoftDelete() {
        if (DeletedAtUtc is not null) {
            throw new InvalidOperationException("La NAV est déjà supprimée.");
        }

        DeletedAtUtc = DateTime.UtcNow;
    }
}