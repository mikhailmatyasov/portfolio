using System;
using System.Collections.Generic;
using System.Linq;
using WeSmart.Alpr.Core.Scheduler.Rules;

namespace WeSmart.Alpr.Core.Scheduler
{
    /// <summary>
    /// Implements a time scheduler
    /// </summary>
    public class DateTimeScheduler : IScheduler
    {
        /// <summary>
        /// Scheduler rules.
        /// </summary>
        public List<ISchedulerRule> Rules { get; private set; } = new List<ISchedulerRule>();

        /// <summary>
        /// Checks if specified date and time is allowed to do something.
        /// </summary>
        /// <param name="date">Specified date and time</param>
        /// <returns>True if specified date and time is allowed.</returns>
        public bool IsAllowed(DateTime date)
        {
            return Rules.Any(x => x.Contains(date));
        }

        public static IScheduler DefaultWeekDaysHourScheduler()
        {
            var scheduler = new DateTimeScheduler
            {
                Rules = new List<ISchedulerRule>
                {
                    new WeekDaysHoursRule { Days = DayOfWeek.All, Hours = Enumerable.Range(0, 24).ToArray() }
                }
            };

            return scheduler;
        }
    }
}