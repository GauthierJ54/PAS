namespace PAS.Asset.Domain.Funds;

public class FundNav {

    public Guid Id { get; private set; }
    public decimal Value { get; private set; }
    public DateTime Date { get; private set; }

    protected FundNav() { }

    public FundNav(decimal value, DateTime date) {
        if (value <= 0) throw new ArgumentOutOfRangeException("La valeur du NAV doit être supérieure à zéro.", nameof(value));
        if (date == default) throw new ArgumentException("La date du NAV ne peut pas être vide.", nameof(date));
        Id = Guid.NewGuid();
        Value = value;
        Date = date;
    }

    public void UpdateValue(decimal newValue) {
        if (newValue <= 0) throw new ArgumentOutOfRangeException("La valeur du NAV doit être supérieure à zéro.", nameof(newValue));
        Value = newValue;
    }
}