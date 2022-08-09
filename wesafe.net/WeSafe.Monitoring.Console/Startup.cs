using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeSafe.Monitoring.Services;

namespace WeSafe.Monitoring.Console
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("api", client =>
            {
                var url = Configuration.GetSection("Api")["Url"];

                client.BaseAddress = new Uri(url);
            });

            services.AddScoped<IApiClient, ApiClient>();
            services.AddScoped<IMonitoringService, MonitoringService>();

            services.Configure<MonitoringOptions>(Configuration.GetSection("Monitoring"));
        }
    }
}