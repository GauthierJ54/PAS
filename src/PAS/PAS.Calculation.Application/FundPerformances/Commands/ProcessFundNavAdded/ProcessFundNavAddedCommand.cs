using MediatR;

namespace PAS.Calculation.Application.FundPerformances.Commands.ProcessFundNavAdded;

public sealed record ProcessFundNavAddedCommand(
    Guid EventId,
    Guid FundId,
    DateOnly Date,
    decimal Value,
    DateTimeOffset OccurredAtUtc,
    int Version) : IRequest;
