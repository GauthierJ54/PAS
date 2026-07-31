using MediatR;
using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds;
using PAS.Asset.Domain.Funds.Events;
using PAS.Common.Exceptions;

namespace PAS.Asset.Application.Funds.Commands.SoftDeleteFundNav {
    public sealed class SoftDeleteFundNavCommandHandler : IRequestHandler<SoftDeleteFundNavCommand> {

        private readonly IFundRepository _fundRepository;
        private readonly IFundNavSoftDeleteEventPublisher _eventPublisher;

        public SoftDeleteFundNavCommandHandler(IFundRepository fundRepository, IFundNavSoftDeleteEventPublisher eventPublisher) {
            _fundRepository = fundRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(SoftDeleteFundNavCommand request, CancellationToken cancellationToken) {
            var fund = await _fundRepository.GetByIdWithNavOfDayAsync(
                request.FundId,
                request.DateTime,
                cancellationToken);

            if (fund is null) {
                throw new NotFoundException(
                    $"Fund with ID '{request.FundId}' does not exist.");
            }

            fund.SoftDeleteNav(request.DateTime);

            var domainEvent = fund.GetDomainEvents().OfType<FundNavSoftDeleteDomainEvent>().Single();
            await _fundRepository.SaveChangesAsync(cancellationToken);
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

            fund.ClearDomainEvents();
        }
    }
}