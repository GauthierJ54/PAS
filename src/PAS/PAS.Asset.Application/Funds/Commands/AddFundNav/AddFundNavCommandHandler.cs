using MediatR;
using PAS.Asset.Application.Abstractions.Messaging;
using PAS.Asset.Domain.Funds;
using PAS.Asset.Domain.Funds.Events;
using PAS.Common.Exceptions;

namespace PAS.Asset.Application.Funds.Commands.AddFundNav {
    public sealed class AddFundNavCommandHandler : IRequestHandler<AddFundNavCommand, Fund?> {

        private readonly IFundRepository _fundRepository;
        private readonly IFundNavAddedOutbox _outbox;

        public AddFundNavCommandHandler(IFundRepository fundRepository, IFundNavAddedOutbox fundNavAddedOutbox) {
            _fundRepository = fundRepository;
            _outbox = fundNavAddedOutbox;
        }

        public async Task<Fund?> Handle(AddFundNavCommand request, CancellationToken cancellationToken) {
            Fund? fund = await _fundRepository.GetByIdWithNavOfDayAsync(request.fundId, request.date, cancellationToken);
            if (fund == null) {
                throw new NotFoundException($"Fund with ID '{request.fundId}' is not exists.");
            }
            fund.AddNav(request.value, request.date);
            var domainEvent = fund.GetDomainEvents().OfType<FundNavAddedDomainEvent>().Single();

            _outbox.Add(domainEvent);
            await _fundRepository.SaveChangesAsync(cancellationToken);

            fund.ClearDomainEvents();

            return fund;
        }
    }
}
