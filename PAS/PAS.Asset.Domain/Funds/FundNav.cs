using PAS.Asset.Domain.Funds.Exceptions;
using PAS.Common;

namespace PAS.Asset.Domain.Funds;

public record FundNav : ValueObject {

    public decimal Value { get; }
    public DateTime Date { get; }

    private FundNav(decimal value, DateTime date) {
        Value = value;
        Date = date;
    }

    public static FundNav Create(decimal value, DateTime date) {
        if (value <= 0) throw new InvalidNavValueException(value);
        if (date == default) throw new ArgumentException("La date du NAV ne peut pas être vide.", nameof(date));

        return new FundNav(value, date);
    }
}