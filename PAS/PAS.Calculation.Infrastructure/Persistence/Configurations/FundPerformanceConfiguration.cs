using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Infrastructure.Persistence.Configurations;

public sealed class FundPerformanceConfiguration
    : IEntityTypeConfiguration<FundPerformance> {
    public void Configure(EntityTypeBuilder<FundPerformance> builder) {
        builder.ToTable("FundPerformances");

        builder.HasKey(f => f.Id);

        // L'identifiant vient de l'événement FundNavAdded.
        // La base ne doit donc pas le générer.
        builder.Property(f => f.Id)
            .ValueGeneratedNever();

        builder.OwnsMany(
            f => f.Navs,
            navBuilder => {
                navBuilder.ToTable("NavPoints");

                navBuilder.WithOwner()
                    .HasForeignKey("FundId");

                navBuilder.Property<Guid>("FundId");

                navBuilder.HasKey(
                    "FundId",
                    nameof(NavPoint.Date));

                navBuilder.Property(n => n.Date)
                    .HasColumnType("date")
                    .IsRequired();

                navBuilder.Property(n => n.Value)
                    .HasPrecision(18, 4)
                    .IsRequired();
            });

        builder.Navigation(f => f.Navs)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}