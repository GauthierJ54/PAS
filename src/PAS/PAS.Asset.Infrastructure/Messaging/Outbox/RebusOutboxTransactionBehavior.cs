using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Infrastructure.Persistence;
using Rebus.Config.Outbox;
using Rebus.Transport;

namespace PAS.Asset.Infrastructure.Messaging.Outbox;

public sealed class RebusOutboxTransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull {
    private readonly AssetDbContext _dbContext;

    public RebusOutboxTransactionBehavior(AssetDbContext dbContext) {
        _dbContext = dbContext;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        if (request is not IOutboxCommand) {
            return await next();
        }

        await using var databaseTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var sqlConnection = (SqlConnection)_dbContext.Database.GetDbConnection();

        var sqlTransaction = (SqlTransaction)databaseTransaction.GetDbTransaction();

        using var rebusTransaction = new RebusTransactionScope();

        rebusTransaction.UseOutbox(sqlConnection, sqlTransaction);

        try {
            var response = await next();

            await rebusTransaction.CompleteAsync();
            await databaseTransaction.CommitAsync(cancellationToken);

            return response;
        } catch {
            await databaseTransaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}