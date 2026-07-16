namespace PAS.Asset.Domain.Funds;

public class Fund : Entity {

    private readonly List<FundNav> _navs = [];
    public string Name { get; private set; } = string.Empty;
    public string Isin { get; private set; } = string.Empty;
    public string Currency { get; private set; } = string.Empty;
    public FundStatus Status { get; private set; }
    public IReadOnlyCollection<FundNav> Navs => _navs;

    private Fund(string name, string isin, string currency) {
        SetId(Guid.NewGuid());
        Name = name;
        Isin = isin;
        Currency = currency;
        Status = FundStatus.Draft;
    }

    public static Fund Create(string name, string isin, string currency) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Le nom du fonds ne peut pas être vide.", nameof(name));
        if (string.IsNullOrWhiteSpace(isin)) throw new ArgumentException("Le code ISIN du fonds ne peut pas être vide.", nameof(isin));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("La devise du fonds ne peut pas être vide.", nameof(currency));

        return new Fund(name, isin, currency);
    }

    public void AddNav(decimal value, DateTime date) {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "La valeur du NAV doit être supérieure à zéro.");
        if (_navs.Any(n => DateTime.Equals(n.Date, date))) throw new InvalidOperationException($"Un NAV pour la date {date.ToShortDateString()} existe déjà.");
        _navs.Add(FundNav.Create(value, date));
    }

    public void PutNav(decimal value, DateTime date) {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "La valeur du NAV doit être supérieure à zéro.");
        if (date == default) throw new ArgumentException("La date du NAV ne peut pas être vide.", nameof(date));
        var existingNav = _navs.FirstOrDefault(n => DateTime.Equals(n.Date, date));
        if (existingNav != null) {
            existingNav.UpdateValue(value);
        } else {
            _navs.Add(FundNav.Create(value, date));
        }
    }

    public void UpdateStatus(FundStatus newStatus) {
        if (newStatus == Status) return;
        Status = newStatus;
    }

    public void UpdateName(string newName) {
        if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("Le nom du fonds ne peut pas être vide.", nameof(newName));
        Name = newName;
    }

    public void UpdateIsin(string newIsin) {
        if (string.IsNullOrWhiteSpace(newIsin)) throw new ArgumentException("Le code ISIN du fonds ne peut pas être vide.", nameof(newIsin));
        Isin = newIsin;
    }

    public void UpdateCurrency(string newCurrency) {
        if (string.IsNullOrWhiteSpace(newCurrency)) throw new ArgumentException("La devise du fonds ne peut pas être vide.", nameof(newCurrency));
        Currency = newCurrency;
    }
}