using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Web;

namespace WeSafe.IntegrationTests.Base
{
    public class MediaGalleryFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "wesafetest-a497d-firebase-adminsdk-vr68w-48206755bd.json");

            return WebHost.CreateDefaultBuilder(null)
                    .ConfigureLogging((context, logging) =>
                    {
                        logging.ClearProviders();

                        var configuration = new LoggingConfiguration();
                        var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
                        configuration.AddTarget(logconsole);

                        configuration.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logconsole);

                        logging.AddNLog(configuration);
                    })
                    .UseStartup<TEntryPoint>();
        }
    }
}
