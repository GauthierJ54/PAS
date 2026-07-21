using Microsoft.EntityFrameworkCore;
using PAS.Calculation.Domain.FundPerformances;
using PAS.Calculation.Infrastructure.Persistence.Inbox;

namespace PAS.Calculation.Infrastructure.Persistence {
    public sealed class CalculationDbContext : DbContext {
        public CalculationDbContext(DbContextOptions<CalculationDbContext> options) : base(options) { }

        public DbSet<FundPerformance> FundPerformances => Set<FundPerformance>();

        public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("calculation");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CalculationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
