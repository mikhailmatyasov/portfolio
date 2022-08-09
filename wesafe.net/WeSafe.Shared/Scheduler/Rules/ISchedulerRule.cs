using System;

namespace WeSmart.Alpr.Core.Scheduler.Rules
{
    /// <summary>
    /// Provides an abstraction for time scheduler rule.
    /// </summary>
    public interface ISchedulerRule
    {
        /// <summary>
        /// Checks if specified date and time is allowed by the rule.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>True if specified date and time is allowed.</returns>
        bool Contains(DateTime date);
    }
}