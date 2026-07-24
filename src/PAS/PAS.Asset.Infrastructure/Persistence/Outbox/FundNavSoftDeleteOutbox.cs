using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds.Events;
using PAS.Contracts.Assets;
using PAS.Contracts.Messaging;
using System.Text.Json;

namespace PAS.Asset.Infrastructure.Persistence.Outbox;

public sealed class FundNavSoftDeleteOutbox : IFundNavSoftDeleteOutbox {
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly AssetDbContext _context;

    public FundNavSoftDeleteOutbox(AssetDbContext context) {
        _context = context;
    }

    public void Add(FundNavSoftDeleteDomainEvent domainEvent) {
        var integrationEvent = new FundNavSoftDeleteIntegrationEvent(domainEvent.EventId, domainEvent.FundId, DateOnly.FromDateTime(domainEvent.Date), new DateTimeOffset(domainEvent.OccurredAtUtc), Version: 1);

        var payload = JsonSerializer.Serialize(integrationEvent, JsonOptions);

        var outboxMessage = OutboxMessage.Create(eventId: integrationEvent.EventId, eventType: nameof(FundNavSoftDeleteIntegrationEvent), routingKey: AssetMessagingTopology.FundNavSoftDeleteRoutingKey, payload: payload, occurredAtUtc: integrationEvent.OccurredAtUtc);

        _context.OutboxMessages.Add(outboxMessage);
    }
}