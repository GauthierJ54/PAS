namespace PAS.Asset.Domain.Funds;

public class FundNav : ValueObject {

    public decimal Value { get; private set; }
    public DateTime Date { get; private set; }

    private FundNav(decimal value, DateTime date) {
        Value = value;
        Date = date;
    }

    public static FundNav Create(decimal value, DateTime date) {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "La valeur du NAV doit être supérieure à zéro.");
        if (date == default) throw new ArgumentException("La date du NAV ne peut pas être vide.", nameof(date));

        return new FundNav(value, date);
    }

    public void UpdateValue(decimal newValue) {
        if (newValue <= 0) throw new ArgumentOutOfRangeException(nameof(newValue), "La valeur du NAV doit être supérieure à zéro.");
        Value = newValue;
    }
}