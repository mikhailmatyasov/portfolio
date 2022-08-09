using Newtonsoft.Json;
using System.Collections.Generic;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public class Queue
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("consumers")]
        public int Consumers { get; set; }

        [JsonProperty("durable")]
        public bool Durable { get; set; }

        [JsonProperty("exclusive")]
        public bool Exclusive { get; set; }

        [JsonProperty("auto_delete")]
        public bool AutoDelete { get; set; }

        [JsonProperty("arguments")]
        public IDictionary<string, object> Arguments { get; set; }

        public Queue() { }

        public Queue(string name, bool durable, bool autoDelete, IDictionary<string, object> args = null)
        {
            Name = name;
            Durable = durable;
            AutoDelete = autoDelete;
            Arguments = args;
        }
    }

    public class DelayedQueue : Queue
    {
        public int Level { get; }

        public DelayedQueue(int level)
        {
            Level = level;
            Name = DelayTopology.GetName(level);

            Durable = true;

            long ttl = 1000;
            for(var i = 0; i < level; i++)
            {
                ttl *= 2;
            }

            Arguments = new Dictionary<string, object>
            {
                { "x-queue-mode", "lazy" },
                { "x-message-ttl", ttl },
                { "x-dead-letter-exchange", DelayTopology.GetName(level - 1) }
            };
        }
    }
}
