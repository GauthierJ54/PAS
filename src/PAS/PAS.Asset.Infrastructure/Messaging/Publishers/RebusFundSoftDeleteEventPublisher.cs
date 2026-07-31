using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds.Events;
using PAS.Contracts.Assets;
using Rebus.Bus;

namespace PAS.Asset.Infrastructure.Messaging.Publishers;

public sealed class RebusFundSoftDeleteEventPublisher : IFundSoftDeleteEventPublisher {
    private readonly IBus _bus;

    public RebusFundSoftDeleteEventPublisher(IBus bus) {
        _bus = bus;
    }

    public async Task PublishAsync(FundSoftDeleteDomainEvent domainEvent, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        var integrationEvent =
            new FundSoftDeleteIntegrationEvent(
                EventId: domainEvent.EventId,
                FundId: domainEvent.FundId,
                OccurredAtUtc: new DateTimeOffset(domainEvent.OccurredAtUtc),
                Version: 1);

        await _bus.Publish(integrationEvent);
    }
}