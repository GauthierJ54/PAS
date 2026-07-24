using MediatR;

namespace PAS.Calculation.Application.FundPerformances.Commands.ProcessFundDelete;

public sealed record ProcessFundDeleteCommand(
    Guid EventId,
    Guid FundId,
    DateTimeOffset OccurredAtUtc,
    int Version) : IRequest;
