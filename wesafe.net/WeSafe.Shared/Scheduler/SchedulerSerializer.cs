using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WeSmart.Alpr.Core.Scheduler.Rules;

namespace WeSmart.Alpr.Core.Scheduler
{
    public class ScheduleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var scheduleType = Type.GetType(jObject["TYPE"].Value<string>());

            IScheduler scheduler;

            if (scheduleType == typeof(DateTimeScheduler))
            {
                scheduler = new DateTimeScheduler();
            }
            else
            {
                throw new NotImplementedException($"Type '{scheduleType.FullName}'.");
            }

            foreach (var jRule in jObject["Rules"])
            {
                var ruleType = Type.GetType(jRule["TYPE"].Value<string>());

                if (ruleType == null)
                {
                    throw new InvalidOperationException($"Type '{jRule["TYPE"].Value<string>()}' not found.");
                }

                var rule = (ISchedulerRule)jRule.ToObject(ruleType);

                scheduler.Rules.Add(rule);
            }

            return scheduler;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = serializer.Converters.Where(s => !(s is ScheduleConverter)).ToList(),
                DateFormatHandling = serializer.DateFormatHandling,
                MissingMemberHandling = serializer.MissingMemberHandling,
                NullValueHandling = serializer.NullValueHandling,
                Formatting = serializer.Formatting
            };
            var localSerializer = JsonSerializer.Create(settings);
            var jObject = JObject.FromObject(value, localSerializer);

            jObject.AddFirst(new JProperty("TYPE", value.GetType().FullName));

            var jRules = jObject["Rules"];

            for (var i = 0; i < jRules.Count(); i++)
            {
                ((JObject)jRules[i]).AddFirst(new JProperty("TYPE", ((IScheduler)value).Rules[i].GetType().FullName));
            }

            jObject.WriteTo(writer);
        }
    }

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