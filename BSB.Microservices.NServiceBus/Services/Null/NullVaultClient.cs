using BSB.Microservices.Vault;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    internal class NullVaultClient : IVaultClient
    {
        public Task<Dictionary<string, object>> GetSecretAsync(string path)
        {
            return Task.FromResult<Dictionary<string, object>>(null);
        }

        public Task<Dictionary<string, object>> GetServiceSecretAsync()
        {
            return Task.FromResult<Dictionary<string, object>>(null);
        }

        public Task<Dictionary<string, object>> TryGetSecretAsync(string path)
        {
            return Task.FromResult<Dictionary<string, object>>(null);
        }

        public Task<Dictionary<string, object>> TryGetServiceSecretAsync()
        {
            return Task.FromResult<Dictionary<string, object>>(null);
        }
    }
}
