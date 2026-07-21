using PAS.Common.DDD;

namespace PAS.Calculation.Domain.FundPerformances {
    public sealed record NavPoint : ValueObject {

        public decimal Value { get; private set; }
        public DateOnly Date { get; private set; }

        private NavPoint(decimal value, DateOnly date) {
            Value = value;
            Date = date;
        }

        public static NavPoint Create(decimal value, DateOnly date) {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "La valeur du NAV doit être supérieure à zéro.");
            if (date == default) throw new ArgumentException("La date du NAV ne peut pas être vide.", nameof(date));
            return new NavPoint(value, date);
        }
    }
}
