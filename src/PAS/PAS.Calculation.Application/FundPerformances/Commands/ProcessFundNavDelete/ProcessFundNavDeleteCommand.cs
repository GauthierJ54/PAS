using MediatR;

namespace PAS.Calculation.Application.FundPerformances.Commands.ProcessFundNavDelete;

public sealed record ProcessFundNavDeleteCommand(
    Guid EventId,
    Guid FundId,
    DateOnly Date,
    DateTimeOffset OccurredAtUtc,
    int Version) : IRequest;
