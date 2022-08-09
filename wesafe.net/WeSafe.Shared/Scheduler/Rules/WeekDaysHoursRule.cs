using System;
using System.Collections.Generic;
using System.Linq;

namespace WeSmart.Alpr.Core.Scheduler.Rules
{
    public class WeekDaysHoursRule : ISchedulerRule
    {
        public DayOfWeek Days { get; set; } = DayOfWeek.All;

        public ICollection<int> Hours { get; set; } = new List<int>();

        public bool Contains(DateTime date)
        {
            return date.IsIn(Days) && Hours.Any(c => date.Hour == c);
        }
    }
}