using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds.Events;
using PAS.Contracts.Assets;
using Rebus.Bus;

namespace PAS.Asset.Infrastructure.Messaging.Publishers;

public sealed class RebusFundNavSoftDeleteEventPublisher : IFundNavSoftDeleteEventPublisher {
    private readonly IBus _bus;

    public RebusFundNavSoftDeleteEventPublisher(IBus bus) {
        _bus = bus;
    }

    public async Task PublishAsync(FundNavSoftDeleteDomainEvent domainEvent, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();

        var integrationEvent =
            new FundNavSoftDeleteIntegrationEvent(
                EventId: domainEvent.EventId,
                FundId: domainEvent.FundId,
                Date: DateOnly.FromDateTime(domainEvent.Date),
                OccurredAtUtc: new DateTimeOffset(domainEvent.OccurredAtUtc),
                Version: 1);

        await _bus.Publish(integrationEvent);
    }
}