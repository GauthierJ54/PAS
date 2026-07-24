using System.Text.Json;
using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds.Events;
using PAS.Contracts.Assets;
using PAS.Contracts.Messaging;

namespace PAS.Asset.Infrastructure.Persistence.Outbox;

public sealed class FundSoftDeleteOutbox : IFundSoftDeleteOutbox {
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly AssetDbContext _context;

    public FundSoftDeleteOutbox(AssetDbContext context) {
        _context = context;
    }

    public void Add(FundSoftDeleteDomainEvent domainEvent) {
        var integrationEvent = new FundSoftDeleteIntegrationEvent(domainEvent.EventId, domainEvent.FundId, new DateTimeOffset(domainEvent.OccurredAtUtc), Version: 1);

        var payload = JsonSerializer.Serialize(integrationEvent, JsonOptions);

        var outboxMessage = OutboxMessage.Create(eventId: integrationEvent.EventId, eventType: nameof(FundSoftDeleteIntegrationEvent), routingKey: AssetMessagingTopology.FundSoftDeleteRoutingKey, payload: payload, occurredAtUtc: integrationEvent.OccurredAtUtc);

        _context.OutboxMessages.Add(outboxMessage);
    }
}