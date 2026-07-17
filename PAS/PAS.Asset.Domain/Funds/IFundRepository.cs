namespace PAS.Asset.Domain.Funds {
    public interface IFundRepository {

        Task AddAsync(Fund fund, CancellationToken cancellationToken);

        Task<bool> ExistsByIsinAsync(string isin, CancellationToken cancellationToken);

        Task<Fund?> GetByIdWithNavOfDayAsync(Guid id, DateTime date, CancellationToken cancellationToken);

        Task UpdateAsync(Fund fund, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);

    }
}
