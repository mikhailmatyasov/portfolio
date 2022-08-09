namespace WeSafe.Logger.MongoDbStorage.Models
{
    public class MongoLogConfiguration
    {
        public const string ConfigurationPosition = "MongoLogConfiguration";

        public string Address { get; set; }

        public string Database { get; set; }

        public string Collection { get; set; }
    }
}
