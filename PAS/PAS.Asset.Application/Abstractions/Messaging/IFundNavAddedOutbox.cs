using PAS.Asset.Domain.Funds.Events;

namespace PAS.Asset.Application.Abstractions.Messaging;

public interface IFundNavAddedOutbox {
    void Add(FundNavAddedDomainEvent domainEvent);
}