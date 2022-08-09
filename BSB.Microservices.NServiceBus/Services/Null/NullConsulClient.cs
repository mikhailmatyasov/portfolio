using BSB.Microservices.Consul;
using Consul;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    internal class NullConsulClient : ICachedConsulClient
    {
        public void Dispose() { }

        public bool GetBoolValue(string key) => default(bool);

        public Task<bool> GetBoolValueAsync(string key) => Task.FromResult(default(bool));

        public int GetIntValue(string key) => default(int);

        public Task<int> GetIntValueAsync(string key) => Task.FromResult(default(int));

        public long GetLongValue(string key) => default(long);

        public Task<long> GetLongValueAsync(string key) => Task.FromResult(default(long));

        public Task<string> GetNodeNameAsync(CancellationToken cancellationToken = default(CancellationToken)) => Task.FromResult(default(string));

        public Task<string> GetServiceConfigValueAsync(string key) => Task.FromResult(default(string));

        public Task<List<KVPair>> GetServiceConfigValuesAsync(string key) => Task.FromResult(default(List<KVPair>));

        public IObservable<string> GetServiceConfigValueStream(string key) => default(IObservable<string>);

        public string GetValue(string key) => default(string);

        public Task<string> GetValueAsync(string key) => Task.FromResult(default(string));

        public List<KVPair> GetValues(string prefix) => default(List<KVPair>);

        public Task<List<KVPair>> GetValuesAsync(string prefix) => Task.FromResult(default(List<KVPair>));

        public IObservable<string> GetValueStream(string key) => default(IObservable<string>);

        public void RegisterServiceFromHealthCheck(string name, HttpContext context, bool registerLocalhost = false) { }

        public Task RegisterServiceFromHealthCheckAsync(string name, HttpContext context, bool registerLocalhost = false) => Task.Run(() => { });


        public bool TryGetValue(string key, out string value)
        {
            value = default(string);
            return default(bool);
        }

#if NET472
        public void RegisterServiceFromHealthCheck(string name, System.Web.HttpContext context, bool registerLocalhost = false) { }

        public Task RegisterServiceFromHealthCheckAsync(string name, System.Web.HttpContext context, bool registerLocalhost = false) => Task.Run(() => { });
#endif
    }
}
