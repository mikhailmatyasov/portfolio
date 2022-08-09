using System;

namespace WeSmart.Alpr.Core.Scheduler
{
    public static class DatetimeExtensions
    {
        /// <summary>
        /// Checks if date of week is in specified days.
        /// </summary>
        /// <param name="date">Checking date.</param>
        /// <param name="days">Days of week in which checking date can be.</param>
        /// <returns>True if checking date is in specified days.</returns>
        public static bool IsIn(this DateTime date, DayOfWeek days)
        {
            switch ( date.DayOfWeek )
            {
                case System.DayOfWeek.Friday:
                    return (days & DayOfWeek.Friday) != 0;
                case System.DayOfWeek.Monday:
                    return (days & DayOfWeek.Monday) != 0;
                case System.DayOfWeek.Saturday:
                    return (days & DayOfWeek.Saturday) != 0;
                case System.DayOfWeek.Sunday:
                    return (days & DayOfWeek.Sunday) != 0;
                case System.DayOfWeek.Thursday:
                    return (days & DayOfWeek.Thursday) != 0;
                case System.DayOfWeek.Tuesday:
                    return (days & DayOfWeek.Tuesday) != 0;
                case System.DayOfWeek.Wednesday:
                    return (days & DayOfWeek.Wednesday) != 0;
                default:
                    throw new NotImplementedException(nameof(date.DayOfWeek));
            }
        }

        /// <summary>
        /// Checks if date is in specified time period.
        /// </summary>
        /// <param name="date">Checking date.</param>
        /// <param name="start">Start of period.</param>
        /// <param name="beginMins">From minutes.</param>
        /// <param name="durationMins">Duration in minutes.</param>
        /// <returns></returns>
        public static bool IsIn(this DateTime date, DateTime start, int beginMins, int durationMins)
        {
            var startToDt = start.AddDays(Math.Floor((date - start).TotalDays));

            return date >= startToDt && date >= startToDt.AddMinutes(beginMins) && date <= startToDt.AddMinutes(beginMins + durationMins);
        }
    }
}