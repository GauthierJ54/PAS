using PAS.Common.Exceptions;

namespace PAS.Asset.Domain.Funds.Exceptions {
    public sealed class FundNavAlreadyExistsException : DomainException {
        public FundNavAlreadyExistsException(DateTime date)
            : base(
                "fund.nav.already_exists",
                $"Une NAV existe déjà pour la date {date:dd/MM/yyyy}.") {
        }
    }
}
