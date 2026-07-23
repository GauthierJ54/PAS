using PAS.Common.Exceptions;

namespace PAS.Asset.Domain.Funds.Exceptions {
    public sealed class InvalidNavValueException : DomainException {
        public InvalidNavValueException(decimal value)
            : base(
                "fund.nav.invalid_value",
                $"La valeur du NAV doit être supérieure à zéro. Valeur reçue : {value}.") {
        }
    }
}
