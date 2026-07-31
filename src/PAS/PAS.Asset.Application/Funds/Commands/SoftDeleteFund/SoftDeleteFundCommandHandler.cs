using MediatR;
using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds;
using PAS.Asset.Domain.Funds.Events;
using PAS.Common.Exceptions;

namespace PAS.Asset.Application.Funds.Commands.SoftDeleteFund {
    public sealed class SoftDeleteFundCommandHandler : IRequestHandler<SoftDeleteFundCommand> {

        private readonly IFundRepository _fundRepository;
        private readonly IFundSoftDeleteEventPublisher _eventPublisher;

        public SoftDeleteFundCommandHandler(IFundRepository fundRepository, IFundSoftDeleteEventPublisher eventPublisher) {
            _fundRepository = fundRepository;
            _eventPublisher = eventPublisher;
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
            await _fundRepository.SaveChangesAsync(cancellationToken);
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

            fund.ClearDomainEvents();
        }
    }
}