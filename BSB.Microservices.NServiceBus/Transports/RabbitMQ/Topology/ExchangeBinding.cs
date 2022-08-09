namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public class ExchangeBinding
    {
        public string Destination { get; set; }

        public string Source { get; set; }

        public string RoutingKey { get; set; }

        public ExchangeBinding() { }

        public ExchangeBinding(string destination, string source, string routingKey)
        {
            Destination = destination;
            Source = source;
            RoutingKey = routingKey;
        }
    }

    public class DelayedExchangeBinding : ExchangeBinding
    {
        public int Level { get; }

        public DelayedExchangeBinding(int level)
        {
            Level = level;

            Destination = DelayTopology.GetName(level - 1);
            Source = DelayTopology.GetName(level);

            var routingKey = "0.#";

            for (var i = level; i < DelayTopology.Levels - 1; i++)
            {
                routingKey = $"*.{routingKey}";
            }

            RoutingKey = routingKey;
        }
    }
}
