using System.Collections.Generic;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public class DelayTopology
    {
        public static int Levels = 28;

        public static string GetName(int level)
        {
            return level == -1
                ? "nsb.delay-delivery"
                : level < 10
                    ? $"nsb.delay-level-0{level}"
                    : $"nsb.delay-level-{level}";
        }

        public static List<DelayedQueue> GetQueues()
        {
            var queues = new List<DelayedQueue>();

            for (var level = 0; level < Levels; level++)
            {
                queues.Add(new DelayedQueue(level));
            }

            return queues;
        }

        public static List<DelayedExchange> GetExchanges()
        {
            var exchanges = new List<DelayedExchange>
            {
                new DelayedExchange(-1)
            };

            for (var level = 0; level < Levels; level++)
            {
                exchanges.Add(new DelayedExchange(level));
            }

            return exchanges;
        }

        public static List<DelayedQueueBinding> GetQueueBindings()
        {
            var bindings = new List<DelayedQueueBinding>();

            for (var level = 0; level < Levels; level++)
            {
                bindings.Add(new DelayedQueueBinding(level));
            }

            return bindings;
        }

        public static List<DelayedExchangeBinding> GetExchangeBindings()
        {
            var bindings = new List<DelayedExchangeBinding>();

            for (var level = 0; level < Levels; level++)
            {
                bindings.Add(new DelayedExchangeBinding(level));
            }

            return bindings;
        }
    }
}
