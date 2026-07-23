using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PAS.Calculation.Infrastructure.Persistence.Inbox;

namespace PAS.Calculation.Infrastructure.Persistence.Configurations;

public sealed class ProcessedMessageConfiguration : IEntityTypeConfiguration<ProcessedMessage> {
    public void Configure(EntityTypeBuilder<ProcessedMessage> builder) {
        builder.ToTable("ProcessedMessages");

        builder.HasKey(message => message.EventId);

        builder.Property(message => message.EventId)
            .ValueGeneratedNever();

        builder.Property(message => message.ProcessedAtUtc)
            .IsRequired();
    }
}