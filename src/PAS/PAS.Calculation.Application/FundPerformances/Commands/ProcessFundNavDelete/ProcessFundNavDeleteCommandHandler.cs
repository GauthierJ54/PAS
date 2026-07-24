using MediatR;
using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Application.FundPerformances.Commands.ProcessFundNavDelete;

public sealed class ProcessFundNavDeleteCommandHandler : IRequestHandler<ProcessFundNavDeleteCommand> {
    private readonly IFundPerformanceRepository _repository;

    public ProcessFundNavDeleteCommandHandler(IFundPerformanceRepository repository) {
        _repository = repository;
    }

    public async Task Handle(ProcessFundNavDeleteCommand request, CancellationToken cancellationToken) {
        var fundPerformance = await _repository.GetByIdAsync(request.FundId, cancellationToken);

        if (fundPerformance is null) {
            throw new InvalidOperationException($"Le fonds avec l'ID {request.FundId} n'existe pas.");
        }

        fundPerformance.DeleteNav(request.Date);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
