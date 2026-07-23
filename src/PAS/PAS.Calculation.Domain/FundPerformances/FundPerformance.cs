using PAS.Common.DDD;

namespace PAS.Calculation.Domain.FundPerformances {
    public class FundPerformance : Entity {

        private readonly List<NavPoint> _navs = [];
        public IReadOnlyCollection<NavPoint> Navs => _navs;

        private FundPerformance(Guid id) {
            if (id == Guid.Empty) throw new ArgumentException("L'identifiant du fonds ne peut pas être vide.", nameof(id));
            SetId(id);
        }

        public static FundPerformance Create(Guid id) {
            return new FundPerformance(id);
        }

        public void AddNav(decimal value, DateOnly date) {
            if (_navs.Any(n => n.Date == date)) throw new InvalidOperationException($"Un NAV pour la date {date} existe déjà.");
            _navs.Add(NavPoint.Create(value, date));
        }

        public void CorrectNav(decimal value, DateOnly date) {
            var index = _navs.FindIndex(n => n.Date == date);

            if (index < 0) {
                throw new InvalidOperationException(
                    $"Aucune NAV n'existe pour la date {date}.");
            }

            var updatedNav = NavPoint.Create(value, date);

            _navs[index] = updatedNav;
        }

        public PerformanceResult? GetDailyPerformance(DateOnly date) {
            var nav = _navs.FirstOrDefault(n => n.Date == date);
            if (nav == null) return null;
            var previousNav = _navs.Where(n => n.Date < date).OrderByDescending(n => n.Date).FirstOrDefault();
            if (previousNav == null) return null;
            var dailyPerformance = (nav.Value - previousNav.Value) / previousNav.Value;
            return new PerformanceResult(date, previousNav.Value, nav.Value, dailyPerformance);
        }
    }
}
