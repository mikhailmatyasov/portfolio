using System;

namespace WeSmart.Alpr.Core.Scheduler.Rules
{
    public class WeekDaysRule : ISchedulerRule
    {
        public DateTime Start { get; set; } = DateTime.MinValue;

        public DateTime End { get; set; } = DateTime.MaxValue;

        public DayOfWeek Days { get; set; } = DayOfWeek.All;

        private int _beginMins = 0;

        public int BeginMins
        {
            get => _beginMins;
            set
            {
                if (value < 0) throw new ArgumentException(nameof(BeginMins));

                _beginMins = value;
            }
        }

        private int _durationMins = 24 * 60;

        public int DurationMins
        {
            get => _durationMins;
            set
            {
                if (value <= 0) throw new ArgumentException(nameof(DurationMins));

                _durationMins = value;
            }
        }

        public bool Contains(DateTime dt)
        {
            return dt.IsIn(Days) && dt >= Start && dt <= End && dt.IsIn(Start, BeginMins, DurationMins);
        }
    }
}