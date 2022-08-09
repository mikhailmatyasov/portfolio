using System;

namespace WeSmart.Alpr.Core.Scheduler.Rules
{
    /// <summary>
    /// Time scheduler rule that allows a week day in time period.
    /// </summary>
    public class MonthDaysRule : ISchedulerRule
    {
        public DateTime Start { get; set; } = DateTime.MinValue;

        public DateTime End { get; set; } = DateTime.MaxValue;

        private int _beginMins = 0;

        public int BeginMins
        {
            get => _beginMins;
            set
            {
                if ( value < 0 ) throw new ArgumentException(nameof(BeginMins));

                _beginMins = value;
            }
        }

        private int _durationMins = 24 * 60;

        public int DurationMins
        {
            get => _durationMins;
            set
            {
                if ( value <= 0 ) throw new ArgumentException(nameof(DurationMins));

                _durationMins = value;
            }
        }

        private int _dayNumber;

        public int DayNumber
        {
            get => _dayNumber;
            set
            {
                if ( value <= 0 || value > 31 ) throw new ArgumentException(nameof(DayNumber));

                _dayNumber = value;
            }
        }

        public bool Contains(DateTime dt)
        {
            return dt >= Start && dt <= End && dt.Day == DayNumber && dt.IsIn(Start, BeginMins, DurationMins);
        }
    }
}