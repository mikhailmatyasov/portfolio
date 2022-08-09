using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using WeSmart.Alpr.Core.Scheduler;
using WeSmart.Alpr.Core.Scheduler.Rules;

namespace WeSafe.Shared
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
}