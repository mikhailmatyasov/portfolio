using System;
using System.Linq;
using TimeZoneConverter;
using WeSafe.DAL.Entities;
using WeSafe.Shared;
using WeSmart.Alpr.Core.Scheduler.Rules;

namespace WeSafe.Services.Extensions
{
    public static class CameraExtensions
    {
        public static bool IsActiveSchedule(this Camera camera, DateTime time, string timeZone)
        {
            bool active = true;

            if (!String.IsNullOrEmpty(camera.Schedule))
            {
                try
                {
                    var scheduler = SchedulerSerializer.Deserialize(camera.Schedule);

                    if ( !String.IsNullOrEmpty(timeZone) )
                    {
                        try
                        {
                            var timeZoneInfo = TZConvert.GetTimeZoneInfo(timeZone);

                            time = TimeZoneInfo.ConvertTimeFromUtc(time, timeZoneInfo);
                        }
                        catch (TimeZoneNotFoundException)
                        {
                            return true;
                        }
                        catch (InvalidTimeZoneException)
                        {
                            return true;
                        }
                    }

                    active = scheduler.IsAllowed(time);
                }
                catch (Exception)
                {
                }
            }

            return active;
        }

        public static bool IsSchedulerEmpty(this Camera camera)
        {
            if ( !String.IsNullOrEmpty(camera.Schedule) )
            {
                var scheduler = SchedulerSerializer.Deserialize(camera.Schedule);

                return scheduler.Rules.Any(c => (c is WeekDaysHoursRule r) && !r.Hours.Any());
            }

            return true;
        }
    }
}
