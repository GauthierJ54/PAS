using Microsoft.EntityFrameworkCore;
using PAS.Asset.Domain.Funds;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PAS.Asset.Infrastructure.Persistence {
    public sealed class AssetDbContext : DbContext {
        public AssetDbContext(DbContextOptions<AssetDbContext> options) : base(options) {
        }

        public DbSet<Fund> Funds => Set<Fund>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("asset");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssetDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
