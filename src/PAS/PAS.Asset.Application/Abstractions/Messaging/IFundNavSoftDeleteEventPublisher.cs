using PAS.Asset.Domain.Funds.Events;

namespace PAS.Asset.Application.Abstractions.Messaging;

public interface IFundNavSoftDeleteEventPublisher {
    Task PublishAsync(FundNavSoftDeleteDomainEvent domainEvent, CancellationToken cancellationToken);
}