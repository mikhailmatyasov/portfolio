using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSB.Microservices.Consul;
using Consul;

namespace BSB.Microservices.NServiceBus
{
    public static class ICachedConsulClientExtensions
    {
        /// <summary>
        /// Retrieves the RabbitMQ connection configuration from Consul
        /// </summary>
        /// <param name="consulClient">The registered <see cref="ICachedConsulClient" /></param>
        /// <param name="serviceIdentity">The service identity.</param>
        /// <returns>
        /// The connection configuration key/values.
        /// </returns>
        public static async Task<Dictionary<string, string>> GetRabbitConfigurationAsync(this ICachedConsulClient consulClient, IServiceIdentity serviceIdentity)
        {
            var kvPairs = await consulClient.GetValuesAsync($"config/{serviceIdentity.ServiceName}/rabbitmq");
            return kvPairs.ToDictionary(k => k.Key.Split('/').Last(), GetStringValue);
        }

        private static string GetStringValue(KVPair kvPair)
        {
            return kvPair.Value?.Length > 0
                ? Encoding.UTF8.GetString(kvPair.Value)
                : null;
        }

        /// <summary>
        /// Gets the persistence connection string..
        /// </summary>
        /// <param name="consulClient">The consul client.</param>
        /// <param name="serviceIdentity">The service identity.</param>
        /// <returns></returns>
        public static string GetPersistenceConnectionString(this ICachedConsulClient consulClient, IServiceIdentity serviceIdentity)
        {
            consulClient.TryGetValue($"config/{serviceIdentity.ServiceName}/connectionstrings/nservicebus/connectionstring", out string cs);
            consulClient.TryGetValue($"config/{serviceIdentity.ServiceName}/connectionstrings/nservicebus/username", out string user);
            consulClient.TryGetValue($"config/{serviceIdentity.ServiceName}/connectionstrings/nservicebus/password", out string pwd);

            return cs?.Replace("{username}", user)?.Replace("{password}", pwd);
        }
    }
}