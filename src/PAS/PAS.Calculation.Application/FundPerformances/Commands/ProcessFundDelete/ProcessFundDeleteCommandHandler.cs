using MediatR;
using PAS.Calculation.Domain.FundPerformances;

namespace PAS.Calculation.Application.FundPerformances.Commands.ProcessFundDelete;

public sealed class ProcessFundDeleteCommandHandler : IRequestHandler<ProcessFundDeleteCommand> {
    private readonly IFundPerformanceRepository _repository;

    public ProcessFundDeleteCommandHandler(IFundPerformanceRepository repository) {
        _repository = repository;
    }

    public async Task Handle(ProcessFundDeleteCommand request, CancellationToken cancellationToken) {
        var fundPerformance = await _repository.GetByIdAsync(request.FundId, cancellationToken);

        if (fundPerformance is null) {
            throw new InvalidOperationException($"Le fonds avec l'ID {request.FundId} n'existe pas.");
        }

        await _repository.DeleteAsync(request.FundId, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
