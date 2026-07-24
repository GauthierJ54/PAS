using PAS.Asset.Domain.Funds.Events;

namespace PAS.Asset.Application.Abstractions.Messaging;

public interface IFundSoftDeleteOutbox {
    void Add(FundSoftDeleteDomainEvent domainEvent);
}