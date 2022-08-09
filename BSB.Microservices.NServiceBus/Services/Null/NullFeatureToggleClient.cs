using BSB.Microservices.Consul;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    internal class NullFeatureToggleClient : IFeatureToggleClient
    {
        public Task<bool> AnyAsync(string feature) => Task.FromResult(default(bool));

        public Task<bool> EnabledAsync(string feature) => Task.FromResult(default(bool));

        public Task<FeatureToggleConfiguration> GetAsync(string feature) => Task.FromResult(default(FeatureToggleConfiguration));

        public Task<bool?> GetCoreLegacyAsync(string feature) => Task.FromResult(default(bool?));

        public Task UpdateAsync(FeatureToggleConfiguration featureToggleConfiguration) => Task.FromResult(default(bool));

        public Task UpdateCoreLegacyAsync(string feature, bool? enabled) => Task.FromResult(default(bool));
    }
}
