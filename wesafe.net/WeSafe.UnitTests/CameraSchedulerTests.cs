using System;
using TimeZoneConverter;
using WeSafe.DAL.Entities;
using WeSafe.Services.Extensions;
using Xunit;

namespace WeSafe.UnitTests
{
    public class CameraSchedulerTests
    {
        [Fact]
        public void Check()
        {
            var camera = new Camera
            {
                Schedule = @"{""TYPE"":""WeSmart.Alpr.Core.Scheduler.DateTimeScheduler"",""Rules"":[{""Hours"":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23],""TYPE"":""WeSmart.Alpr.Core.Scheduler.Rules.WeekDaysHoursRule"",""Days"":64}]}"
            };

            var tz = TZConvert.GetTimeZoneInfo("Asia/Jerusalem");

            Assert.False(camera.IsActiveSchedule(new DateTime(2020, 11, 2, 6, 0, 0, DateTimeKind.Utc), tz.Id));
        }
    }
}
