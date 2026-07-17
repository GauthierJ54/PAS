using PAS.Asset.Domain.Common;
using PAS.Asset.Domain.Funds.Events;
using PAS.Asset.Domain.Funds.Exceptions;
using PAS.Common;

namespace PAS.Asset.Domain.Funds;

public class Fund : Entity {

    private readonly List<FundNav> _navs = [];
    private readonly List<IDomainEvent> _domainEvents = [];
    public string Name { get; private set; } = string.Empty;
    public string Isin { get; private set; } = string.Empty;
    public string Currency { get; private set; } = string.Empty;
    public FundStatus Status { get; private set; }
    public IReadOnlyCollection<FundNav> Navs => _navs;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

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

        var fund = new Fund(name, isin, currency);
        return fund;
    }

    public void AddNav(decimal value, DateTime date) {
        if (_navs.Any(n => DateTime.Equals(n.Date, date))) throw new FundNavAlreadyExistsException(date);
        _navs.Add(FundNav.Create(value, date));
        _domainEvents.Add(new FundNavAddedDomainEvent(Id, date, value));
    }

    public void UpdateNav(decimal value, DateTime date) {
        FundNav insertNav = FundNav.Create(value, date);
        var existingNav = _navs.FirstOrDefault(n => DateTime.Equals(n.Date, date));
        if (existingNav != null) _navs.Remove(existingNav);
        _navs.Add(insertNav);
        _domainEvents.Add(new FundNavUpdatedDomainEvent(Id, date, value));
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

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();
}