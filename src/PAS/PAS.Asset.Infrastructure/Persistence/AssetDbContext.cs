using Microsoft.EntityFrameworkCore;
using PAS.Asset.Domain.Funds;
using PAS.Asset.Infrastructure.Persistence.Outbox;

namespace PAS.Asset.Infrastructure.Persistence {
    public sealed class AssetDbContext : DbContext {
        public AssetDbContext(DbContextOptions<AssetDbContext> options) : base(options) {
        }

        public DbSet<Fund> Funds => Set<Fund>();

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("asset");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssetDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
