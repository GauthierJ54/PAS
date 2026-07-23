using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PAS.Asset.Domain.Funds;

namespace PAS.Asset.Infrastructure.Persistence.Configurations {
    public sealed class FundConfiguration : IEntityTypeConfiguration<Fund> {
        public void Configure(EntityTypeBuilder<Fund> builder) {
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Name).IsRequired().HasMaxLength(200);
            builder.Property(f => f.Isin).IsRequired().HasMaxLength(12);
            builder.HasIndex(f => f.Isin).IsUnique();
            builder.Property(f => f.Currency).IsRequired().HasMaxLength(3);
            builder.Property(f => f.Status).IsRequired();
            builder.Property(f => f.DeletedAtUtc);
            builder.HasMany(f => f.Navs).WithOne().HasForeignKey("FundId").IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(f => f.Navs).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
