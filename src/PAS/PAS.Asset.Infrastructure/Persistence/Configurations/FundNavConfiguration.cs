using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PAS.Asset.Domain.Funds;

namespace PAS.Asset.Infrastructure.Persistence.Configurations;

public sealed class FundNavConfiguration : IEntityTypeConfiguration<FundNav> {
    public void Configure(EntityTypeBuilder<FundNav> builder) {
        builder.ToTable("FundNav");

        // Clé technique uniquement connue d'EF Core
        builder.Property<long>("Id").ValueGeneratedOnAdd();

        builder.HasKey("Id");

        builder.Property<Guid>("FundId").IsRequired();

        builder.Property(nav => nav.Date).HasColumnType("date").IsRequired();

        builder.Property(nav => nav.Value).HasPrecision(18, 4).IsRequired();

        builder.Property(nav => nav.DeletedAtUtc);

        // Une seule NAV active pour un fonds et une date
        builder.HasIndex("FundId",nameof(FundNav.Date)).IsUnique().HasDatabaseName("UX_FundNav_FundId_Date_Active").HasFilter("[DeletedAtUtc] IS NULL");
    }
}