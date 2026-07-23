namespace PAS.Common.DDD {
    public interface ISoftDeletable {
        DateTime? DeletedAtUtc { get; }
    }
}
