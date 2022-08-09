using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace WeSafe.Monitoring.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                var host = CreateHostBuilder(args);

                await host.RunConsoleAsync();
            }
            catch ( Exception ex )
            {
                // NLog: catch any exception and log it.
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureLogging((context, loggingBuilder) =>
                       {
                           loggingBuilder.ClearProviders();
                           loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                           loggingBuilder.AddNLog(context.Configuration);
                       })
                       .ConfigureServices((context, services) =>
                       {
                           var startup = new Startup(context.Configuration, context.HostingEnvironment);

                           startup.ConfigureServices(services);

                           services.AddHostedService<MonitoringHostService>();
                       });
        }
    }
}
