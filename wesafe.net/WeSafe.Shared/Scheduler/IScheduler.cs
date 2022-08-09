using System;
using System.Collections.Generic;
using WeSmart.Alpr.Core.Scheduler.Rules;

namespace WeSmart.Alpr.Core.Scheduler
{
    /// <summary>
    /// Provides an abstraction for time scheduler.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Scheduler rules.
        /// </summary>
        List<ISchedulerRule> Rules { get; }

        /// <summary>
        /// Checks if specified date and time is allowed.
        /// </summary>
        /// <param name="date">Specified date and time</param>
        /// <returns>True if specified date and time is allowed.</returns>
        bool IsAllowed(DateTime date);
    }
}