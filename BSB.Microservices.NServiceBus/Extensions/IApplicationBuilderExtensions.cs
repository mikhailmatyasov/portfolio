using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    public static class IApplicationBuilderExtensions
    {
        private interface INullBus : IBus { }

        public static IApplicationBuilder UseNServiceBus<TBus>(this IApplicationBuilder app)
            where TBus : IBus
        {
            return app.UseNServiceBusAsync<TBus, INullBus, INullBus>().ConfigureAwait(true).GetAwaiter().GetResult();
        }

        public static IApplicationBuilder UseNServiceBus<TBus1, TBus2>(this IApplicationBuilder app)
            where TBus1 : IBus
            where TBus2 : IBus
        {
            return app.UseNServiceBusAsync<TBus1, TBus2, INullBus>().ConfigureAwait(true).GetAwaiter().GetResult();
        }

        public static IApplicationBuilder UseNServiceBus<TBus1, TBus2, TBus3>(this IApplicationBuilder app)
            where TBus1 : IBus
            where TBus2 : IBus
            where TBus3 : IBus
        {
            return app.UseNServiceBusAsync<TBus1, TBus2, TBus3>().ConfigureAwait(true).GetAwaiter().GetResult();
        }

        public static async Task<IApplicationBuilder> UseNServiceBusAsync<TBus1>(this IApplicationBuilder app)
            where TBus1 : IBus
        {
            return await app.UseNServiceBusAsync<TBus1, INullBus, INullBus>();
        }

        public static async Task<IApplicationBuilder> UseNServiceBusAsync<TBus1, TBus2>(this IApplicationBuilder app)
            where TBus1 : IBus
            where TBus2 : IBus
        {
            return await app.UseNServiceBusAsync<TBus1, TBus2, INullBus>();
        }

        public static async Task<IApplicationBuilder> UseNServiceBusAsync<TBus1, TBus2, TBus3>(this IApplicationBuilder app)
            where TBus1 : IBus
            where TBus2 : IBus
            where TBus3 : IBus
        {
            var loggerFactory = app.ApplicationServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

            if(loggerFactory != null)
            {
                LogManager.Use<MicrosoftLogFactory>().UseMsFactory(loggerFactory);
            }

            if (typeof(TBus1) != typeof(INullBus))
            {
                await app.ApplicationServices.GetRequiredService<TBus1>().StartAsync();
            }

            if (typeof(TBus2) != typeof(INullBus))
            {
                await app.ApplicationServices.GetRequiredService<TBus2>().StartAsync();
            }

            if (typeof(TBus3) != typeof(INullBus))
            {
                await app.ApplicationServices.GetRequiredService<TBus3>().StartAsync();
            }

            return app;
        }

        public static IApplicationBuilder UseTopicReceiveBus(this IApplicationBuilder app, string topic, string endpointSuffix)
        {
            var loggerFactory = app.ApplicationServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

            if (loggerFactory != null)
            {
                LogManager.Use<MicrosoftLogFactory>().UseMsFactory(loggerFactory);
            }

            var container = app.ApplicationServices.GetRequiredService<ITopicBusContainer>();

            container.GetOrAdd(topic, endpointSuffix).StartAsync().ConfigureAwait(true).GetAwaiter().GetResult();

            return app;
        }

        public static async Task<IApplicationBuilder> UseTopicReceiveBusAsync(this IApplicationBuilder app, string topic, string endpointSuffix)
        {
            var loggerFactory = app.ApplicationServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

            if (loggerFactory != null)
            {
                LogManager.Use<MicrosoftLogFactory>().UseMsFactory(loggerFactory);
            }

            var container = app.ApplicationServices.GetRequiredService<ITopicBusContainer>();

            await container.GetOrAdd(topic, endpointSuffix).StartAsync();

            return app;
        }
    }
}
