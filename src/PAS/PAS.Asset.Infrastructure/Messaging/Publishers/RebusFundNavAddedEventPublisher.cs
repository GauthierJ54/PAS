using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds.Events;
using PAS.Contracts.Assets;
using Rebus.Bus;

namespace PAS.Asset.Infrastructure.Messaging.Publishers;

public sealed class RebusFundNavAddedEventPublisher : IFundNavAddedEventPublisher {
    private readonly IBus _bus;

    public RebusFundNavAddedEventPublisher(IBus bus) {
        _bus = bus;
    }

    public async Task PublishAsync(FundNavAddedDomainEvent domainEvent, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        var integrationEvent =
            new FundNavAddedIntegrationEvent(
                EventId: domainEvent.EventId,
                FundId: domainEvent.FundId,
                Date: DateOnly.FromDateTime(domainEvent.Date),
                Value: domainEvent.Value,
                OccurredAtUtc: new DateTimeOffset(domainEvent.OccurredAtUtc),
                Version: 1);

        await _bus.Publish(integrationEvent);
    }
}