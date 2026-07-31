using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PAS.Calculation.Application.FundPerformances.Commands.ProcessFundDelete;
using PAS.Calculation.Infrastructure.Persistence;
using PAS.Calculation.Infrastructure.Persistence.Inbox;
using PAS.Contracts.Assets;
using Rebus.Handlers;

namespace PAS.Calculation.Infrastructure.Messaging.Handlers;

public sealed class FundSoftDeleteIntegrationEventHandler : IHandleMessages<FundSoftDeleteIntegrationEvent> {
    private readonly CalculationDbContext _dbContext;
    private readonly ISender _sender;
    private readonly ILogger<FundSoftDeleteIntegrationEventHandler> _logger;

    public FundSoftDeleteIntegrationEventHandler(CalculationDbContext dbContext, ISender sender, ILogger<FundSoftDeleteIntegrationEventHandler> logger) {
        _dbContext = dbContext;
        _sender = sender;
        _logger = logger;
    }

    public async Task Handle(FundSoftDeleteIntegrationEvent message) {
        Validate(message);

        var alreadyProcessed = await _dbContext.ProcessedMessages.AsNoTracking().AnyAsync(processed => processed.EventId == message.EventId);

        if (alreadyProcessed) {
            _logger.LogInformation("L'événement Rebus {EventId} a déjà été traité.", message.EventId);
            return;
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync();

        try {
            var command = new ProcessFundDeleteCommand(
                message.EventId,
                message.FundId,
                message.OccurredAtUtc,
                message.Version);

            await _sender.Send(command);

            var processedMessage = ProcessedMessage.Create(message.EventId);

            _dbContext.ProcessedMessages.Add(processedMessage);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Suppression Rebus reçue pour le fonds {FundId}, événement {EventId}, survenue à {OccurredAtUtc}.", message.FundId, message.EventId, message.OccurredAtUtc);
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static void Validate(FundSoftDeleteIntegrationEvent message) {
        if (message.EventId == Guid.Empty) {
            throw new InvalidOperationException(
                "EventId ne peut pas être vide.");
        }

        if (message.FundId == Guid.Empty) {
            throw new InvalidOperationException(
                "FundId ne peut pas être vide.");
        }

        if (message.OccurredAtUtc == default) {
            throw new InvalidOperationException(
                "OccurredAtUtc est obligatoire.");
        }

        if (message.Version != 1) {
            throw new InvalidOperationException(
                $"Version FundSoftDelete non supportée : {message.Version}.");
        }
    }
}