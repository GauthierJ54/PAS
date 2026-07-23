using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PAS.Asset.Infrastructure.Persistence.Outbox;

namespace PAS.Asset.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage> {
    public void Configure(EntityTypeBuilder<OutboxMessage> builder) {
        builder.ToTable("OutboxMessages");

        builder.HasKey(message => message.EventId);

        builder.Property(message => message.EventId)
            .ValueGeneratedNever();

        builder.Property(message => message.EventType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(message => message.RoutingKey)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(message => message.Payload)
            .IsRequired();

        builder.Property(message => message.OccurredAtUtc)
            .IsRequired();

        builder.Property(message => message.ProcessedAtUtc);

        builder.Property(message => message.RetryCount)
            .IsRequired();

        builder.Property(message => message.LastError)
            .HasMaxLength(2000);

        builder.HasIndex(message => message.ProcessedAtUtc);
    }
}