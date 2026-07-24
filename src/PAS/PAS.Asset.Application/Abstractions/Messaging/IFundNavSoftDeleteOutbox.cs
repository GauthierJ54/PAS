using PAS.Asset.Domain.Funds.Events;

namespace PAS.Asset.Application.Abstractions.Messaging;

public interface IFundNavSoftDeleteOutbox {
    void Add(FundNavSoftDeleteDomainEvent domainEvent);
}