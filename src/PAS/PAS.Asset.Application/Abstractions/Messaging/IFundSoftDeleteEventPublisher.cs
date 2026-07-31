using PAS.Asset.Domain.Funds.Events;

namespace PAS.Asset.Application.Abstractions.Messaging;

public interface IFundSoftDeleteEventPublisher {
    Task PublishAsync(FundSoftDeleteDomainEvent domainEvent, CancellationToken cancellationToken);
}