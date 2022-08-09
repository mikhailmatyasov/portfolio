using System;

namespace WeSmart.Alpr.Core.Scheduler.Rules
{
    /// <summary>
    /// Time scheduler rule that allows a specified date and time in time period.
    /// </summary>
    public class OneTimeRule : ISchedulerRule
    {
        public DateTime Start { get; set; } = DateTime.MinValue;

        public DateTime End { get; set; } = DateTime.MaxValue;

        public bool Contains(DateTime dt)
        {
            return dt >= Start && dt <= End;
        }
    }
}