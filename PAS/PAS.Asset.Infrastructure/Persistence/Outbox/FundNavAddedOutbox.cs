using System.Text.Json;
using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds.Events;
using PAS.Contracts.Assets;
using PAS.Contracts.Messaging;

namespace PAS.Asset.Infrastructure.Persistence.Outbox;

public sealed class FundNavAddedOutbox : IFundNavAddedOutbox {
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly AssetDbContext _context;

    public FundNavAddedOutbox(AssetDbContext context) {
        _context = context;
    }

    public void Add(FundNavAddedDomainEvent domainEvent) {
        var integrationEvent = new FundNavAddedIntegrationEvent(domainEvent.EventId, domainEvent.FundId, DateOnly.FromDateTime(domainEvent.Date), domainEvent.Value, new DateTimeOffset(domainEvent.OccurredAtUtc), Version: 1);

        var payload = JsonSerializer.Serialize(integrationEvent, JsonOptions);

        var outboxMessage = OutboxMessage.Create(eventId: integrationEvent.EventId, eventType: nameof(FundNavAddedIntegrationEvent), routingKey: AssetMessagingTopology.FundNavAddedRoutingKey, payload: payload, occurredAtUtc: integrationEvent.OccurredAtUtc);

        _context.OutboxMessages.Add(outboxMessage);
    }
}