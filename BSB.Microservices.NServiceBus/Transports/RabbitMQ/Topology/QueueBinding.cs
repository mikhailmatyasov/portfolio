namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public class QueueBinding
    {
        public string QueueName { get; set; }

        public string ExchangeName { get; set; }

        public string RoutingKey { get; set; }

        public QueueBinding() { }

        public QueueBinding(string queueName, string exchangeName, string routingKey)
        {
            QueueName = queueName;
            ExchangeName = exchangeName;
            RoutingKey = routingKey;
        }
    }

    public class DelayedQueueBinding : QueueBinding
    {
        public int Level { get; }

        public DelayedQueueBinding(int level)
        {
            Level = level;

            QueueName = DelayTopology.GetName(level);
            ExchangeName = DelayTopology.GetName(level);

            var routingKey = "1.#";

            for(var i = level; i < DelayTopology.Levels - 1; i++)
            {
                routingKey = $"*.{routingKey}";
            }

            RoutingKey = routingKey;
        }
    }
}
