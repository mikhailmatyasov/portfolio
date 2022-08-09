namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public class Exchange
    {
        public string Name { get; set; }

        public string ExchangeType { get; set; }

        public bool Durable { get; set; }

        public bool AutoDelete { get; set; }

        public Exchange() { }

        public Exchange(string name, string exchangeType, bool durable, bool autoDelete = false)
        {
            Name = name;
            ExchangeType = exchangeType;
            Durable = durable;
            AutoDelete = autoDelete;
        }
    }

    public class DelayedExchange : Exchange
    {
        public int Level { get; }

        public DelayedExchange(int level)
        {
            Level = level;
            Name = DelayTopology.GetName(level);
            ExchangeType = "topic";
            Durable = true;
        }
    }
}
