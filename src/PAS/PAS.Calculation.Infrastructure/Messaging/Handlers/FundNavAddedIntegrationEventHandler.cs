using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PAS.Calculation.Application.FundPerformances.Commands.ProcessFundNavAdded;
using PAS.Calculation.Infrastructure.Persistence;
using PAS.Calculation.Infrastructure.Persistence.Inbox;
using PAS.Contracts.Assets;
using Rebus.Handlers;

namespace PAS.Calculation.Infrastructure.Messaging.Handlers;

public sealed class FundNavAddedIntegrationEventHandler : IHandleMessages<FundNavAddedIntegrationEvent> {
    private readonly CalculationDbContext _dbContext;
    private readonly ISender _sender;
    private readonly ILogger<FundNavAddedIntegrationEventHandler> _logger;

    public FundNavAddedIntegrationEventHandler(CalculationDbContext dbContext, ISender sender, ILogger<FundNavAddedIntegrationEventHandler> logger) {
        _dbContext = dbContext;
        _sender = sender;
        _logger = logger;
    }

    public async Task Handle(FundNavAddedIntegrationEvent message) {
        Validate(message);

        var alreadyProcessed = await _dbContext.ProcessedMessages.AsNoTracking().AnyAsync(processed => processed.EventId == message.EventId);

        if (alreadyProcessed) {
            _logger.LogInformation("L'événement Rebus {EventId} a déjà été traité.", message.EventId);
            return;
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync();

        try {
            var command = new ProcessFundNavAddedCommand(
                message.EventId,
                message.FundId,
                message.Date,
                message.Value,
                message.OccurredAtUtc,
                message.Version);

            await _sender.Send(command);

            var processedMessage = ProcessedMessage.Create(message.EventId);

            _dbContext.ProcessedMessages.Add(processedMessage);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("NAV Rebus reçue pour le fonds {FundId}, " + "date {Date}, événement {EventId}.", message.FundId, message.Date, message.EventId);
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static void Validate(FundNavAddedIntegrationEvent message) {
        if (message.EventId == Guid.Empty) {
            throw new InvalidOperationException(
                "EventId ne peut pas être vide.");
        }

        if (message.FundId == Guid.Empty) {
            throw new InvalidOperationException(
                "FundId ne peut pas être vide.");
        }

        if (message.Date == default) {
            throw new InvalidOperationException(
                "La date de la NAV est obligatoire.");
        }

        if (message.Value <= 0) {
            throw new InvalidOperationException(
                "La valeur de la NAV doit être positive.");
        }

        if (message.OccurredAtUtc == default) {
            throw new InvalidOperationException(
                "OccurredAtUtc est obligatoire.");
        }

        if (message.Version != 1) {
            throw new InvalidOperationException(
                $"Version FundNavAdded non supportée : {message.Version}.");
        }
    }
}