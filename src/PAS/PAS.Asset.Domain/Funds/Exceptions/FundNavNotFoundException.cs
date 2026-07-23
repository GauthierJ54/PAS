using PAS.Common.Exceptions;

namespace PAS.Asset.Domain.Funds.Exceptions {
    public sealed class FundNavNotFoundException : DomainException {
        public FundNavNotFoundException(DateTime date)
            : base(
                "fund.nav.not_found",
                $"Il n'y a pas de NAV pour la date {date:dd/MM/yyyy}.") {
        }
    }
}
