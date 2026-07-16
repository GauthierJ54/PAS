namespace PAS.Asset.Domain.Funds {
    public interface IFundRepository {

        Task AddAsync(Fund fund, CancellationToken cancellationToken);

        Task<bool> ExistsByIsinAsync(string isin, CancellationToken cancellationToken);

        Task<Fund?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task UpdateAsync(Fund fund, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);

    }
}
