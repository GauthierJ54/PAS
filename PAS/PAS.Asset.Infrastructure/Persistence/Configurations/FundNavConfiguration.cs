using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PAS.Asset.Domain.Funds;

namespace PAS.Asset.Infrastructure.Persistence.Configurations {
    public sealed class FundNavConfiguration : IEntityTypeConfiguration<FundNav> {
        public void Configure(EntityTypeBuilder<FundNav> builder) {
            builder.Property<Guid>("FundId");

            builder.HasKey("FundId", nameof(FundNav.Date));

            builder.Property(n => n.Date)
                .IsRequired();

            builder.Property(n => n.Value)
                .HasPrecision(18, 4)
                .IsRequired();
        }
    }
}
