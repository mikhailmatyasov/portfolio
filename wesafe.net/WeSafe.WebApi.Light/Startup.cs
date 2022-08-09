using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using WeSafe.DAL;
using WeSafe.Nano.DAL;
using WeSafe.Nano.Services;
using WeSafe.Nano.Services.Abstraction.Abstraction.Services;
using WeSafe.Nano.Services.Stubs;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core;

namespace WeSafe.Nano.WebApi
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddMvc().AddApplicationPart(typeof(BaseStartup).Assembly);

            services.AddScoped<WeSafeDbContext>(provider => provider.GetRequiredService<WeSafeNanoDbContext>());

            //services.AddTransient<IAuthorizationHandler, WhiteListIpHandler>();
            services.AddSingleton<ICloudStorage, StubGoogleCloudStorage>();
            //services.AddSingleton<IEmailSender, EmailSender>();
            //services.AddScoped<IPermittedAdminIpService, PermittedAdminIpService>();
            //services.AddScoped<ITelegramService, TelegramService>();
            //services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMobileService, StubMobileService>();
            services.AddScoped<ICleanupLogsService, CleanupLogsService>();
            services.AddScoped<IVideoRecordsService, VideoRecordsService>();
            services.AddScoped<IClientService, NanoClientService>();
            //services.AddScoped<IUnhandledExceptionsSender, UnhandledExceptionsSender>();
            //services.AddScoped<IUnhandledExceptionsService, UnhandledExceptionsService>();
            //services.AddSingleton<UnhandledExceptionsHandlerHostedService>();
            services.AddSingleton<ITelegramClient, StubNanoTelegramClient>();
            //services.AddSingleton<EmailMapper>();
            //services.AddSingleton<UnhandledExceptionFilterMapper>();
            //services.AddSingleton<PermittedAdminIpMapper>();

            services.Configure<PhysicalFileStorageOptions>(Configuration.GetSection("PhysicalFileStorage"));
            services.Configure<CleanupLogsOptions>(Configuration.GetSection("CleanupLogs"));

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";//"ClientApp/dist/ClientApp";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "Content")),
                RequestPath = "/Content"
            });

            app.UseSpaStaticFiles();

            base.Configure(app, env);

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                //spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    //spa.UseAngularCliServer(npmScript: "start");
                }
            });

            MigrateAndSeed(app);
        }

        protected override void AddDbContext(IServiceCollection serviceCollection)
        {
            var connectionString = Configuration.GetConnectionString("DB_CONNECTION_STRING");

            serviceCollection.AddDbContext<WeSafeNanoDbContext>(options =>
                options.UseSqlite(connectionString,
                    b => b.MigrationsAssembly(typeof(WeSafeNanoDbContext).Assembly.FullName)));
        }

        protected override void AddHostedServices(IServiceCollection services)
        {
            base.AddHostedServices(services);

            services.AddHostedService<ClearDeviceLogsService>();
            services.AddHostedService<ClearCameraLogsService>();
        }
    }
}
