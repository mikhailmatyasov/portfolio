using Newtonsoft.Json;
using System.Collections.Generic;
using WeSmart.Alpr.Core.Scheduler;

namespace WeSafe.Shared
{
    public static class SchedulerSerializer
    {
        private static JsonSerializerSettings _settings;

        private static JsonSerializerSettings GetSettings()
        {
            return _settings = _settings ?? new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new ScheduleConverter()
                }
            };
        }

        public static string Serialize(this IScheduler scheduler)
        {
            return JsonConvert.SerializeObject(scheduler, GetSettings());
        }

        public static IScheduler Deserialize(string serializeString)
        {
            return JsonConvert.DeserializeObject<IScheduler>(serializeString, GetSettings());
        }
    }
}
