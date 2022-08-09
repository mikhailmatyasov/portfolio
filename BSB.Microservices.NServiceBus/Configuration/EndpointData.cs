namespace BSB.Microservices.NServiceBus
{
    public class EndpointData
    {
        public EndpointType Type { get; private set; }

        /// <summary>
        /// Pattern used to filter subscriptions
        /// * (star) substitutes exactly one word
        /// # (hash) substitutes zero or more words
        /// examples:
        ///     big.#
        ///     big.fat.*
        ///     big.fat.cat
        /// </summary>
        public string TopicPattern { get; private set; }

        /// <summary>
        /// Queue Name Suffix
        /// </summary>
        public string Suffix { get; private set; }

        public static EndpointData SendOnly()
        {
            return new EndpointData
            {
                Type = EndpointType.SendOnly,
                TopicPattern = string.Empty,
                Suffix = string.Empty
            };
        }

        public static EndpointData Default()
        {
            return new EndpointData
            {
                Type = EndpointType.Default,
                TopicPattern = string.Empty,
                Suffix = string.Empty
            };
        }

        public static EndpointData Topic(string topic, string endpointSuffix)
        {
            return new EndpointData
            {
                Type = EndpointType.Topic,
                TopicPattern = topic,
                Suffix = endpointSuffix
            };
        }
    }
}
