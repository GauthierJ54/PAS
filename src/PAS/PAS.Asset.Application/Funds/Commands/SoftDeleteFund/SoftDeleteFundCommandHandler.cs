using MediatR;
using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds;
using PAS.Asset.Domain.Funds.Events;
using PAS.Common.Exceptions;

namespace PAS.Asset.Application.Funds.Commands.SoftDeleteFund {
    public sealed class SoftDeleteFundCommandHandler : IRequestHandler<SoftDeleteFundCommand> {

        private readonly IFundRepository _fundRepository;
        private readonly IFundSoftDeleteOutbox _outbox;

        public SoftDeleteFundCommandHandler(IFundRepository fundRepository, IFundSoftDeleteOutbox fundSoftDeleteOutbox) {
            _fundRepository = fundRepository;
            _outbox = fundSoftDeleteOutbox;
        }

        public async Task Handle(SoftDeleteFundCommand request, CancellationToken cancellationToken) {
            var fund = await _fundRepository.GetByIdAsync(
                request.FundId,
                cancellationToken);

            if (fund is null) {
                throw new NotFoundException(
                    $"Fund with ID '{request.FundId}' does not exist.");
            }
            fund.SoftDelete();
            var domainEvent = fund.GetDomainEvents().OfType<FundSoftDeleteDomainEvent>().Single();
            _outbox.Add(domainEvent);

            await _fundRepository.SaveChangesAsync(cancellationToken);
        }
    }
}