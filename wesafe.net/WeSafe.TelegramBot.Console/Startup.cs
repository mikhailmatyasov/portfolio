using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeSafe.TelegramBot.Services;

namespace WeSafe.TelegramBot.NetCore
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
            services.AddScoped<ITelegramHandleUpdate, TelegramHandleUpdate>();
            services.AddHttpClient("api", client =>
            {
                var url = Configuration.GetSection("Api")["Url"];

                client.BaseAddress = new Uri(url);
            });
            services.AddScoped<IApiClient, ApiClient>();
            services.AddScoped<IFileStorage, PhysicalFileStorage>();

            services.AddTransient<StartCommand>();
            services.AddTransient<RegisterCommand>();
            services.AddTransient<SystemMenuHandler>();
            services.AddTransient<ViewAllHandler>();
            services.AddTransient<ViewHandler>();
            services.AddTransient<DeviceStatHandler>();
            services.AddTransient<DeviceSettingsHandler>();
            services.AddTransient<MuteSystemHandler>();
            services.AddTransient<DeviceArmHandler>();
            services.AddTransient<CameraSettingsHandler>();
            services.AddTransient<MuteCameraHandler>();
            services.AddTransient<CameraConfidenceHandler>();
            services.AddTransient<CameraSensitivityHandler>();

            services.Configure<PhysicalFileStorageOptions>(Configuration.GetSection("PhysicalFileStorage"));
        }
    }
}