using MediatR;
using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Application.FundPerformances.Commands.ProcessFundNavAdded;

public sealed class ProcessFundNavAddedCommandHandler
    : IRequestHandler<ProcessFundNavAddedCommand>
{
    private readonly IFundPerformanceRepository _repository;

    public ProcessFundNavAddedCommandHandler(IFundPerformanceRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        ProcessFundNavAddedCommand request,
        CancellationToken cancellationToken)
    {
        var fundPerformance = await _repository.GetByIdAsync(
            request.FundId,
            cancellationToken);

        if (fundPerformance is null)
        {
            fundPerformance = FundPerformance.Create(request.FundId);
            await _repository.AddAsync(fundPerformance, cancellationToken);
        }

        fundPerformance.AddNav(request.Value, request.Date);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
