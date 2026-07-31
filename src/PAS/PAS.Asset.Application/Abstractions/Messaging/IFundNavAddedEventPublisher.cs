using PAS.Asset.Domain.Funds.Events;

namespace PAS.Asset.Application.Abstractions.Messaging;

public interface IFundNavAddedEventPublisher {
    Task PublishAsync(FundNavAddedDomainEvent domainEvent, CancellationToken cancellationToken);
}