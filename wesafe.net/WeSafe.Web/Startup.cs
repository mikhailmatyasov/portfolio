using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Net.Http;
using WeSafe.Services;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core;
using WeSafe.Web.Core.Mappers;
using WeSafe.Web.Policies.RequirementHandler;
using WeSafe.Web.Policies.Requirements;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WeSafe.Web
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddTransient<IAuthorizationHandler, WhiteListIpHandler>();
            services.AddSingleton<ICloudStorage, GoogleCloudStorage>();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddScoped<IPermittedAdminIpService, PermittedAdminIpService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMobileService, MobileService>();
            services.AddScoped<IUnhandledExceptionsSender, UnhandledExceptionsSender>();
            services.AddScoped<IUnhandledExceptionsService, UnhandledExceptionsService>();
            services.AddScoped<ICleanupLogsService, CleanupLogsService>();
            services.AddSingleton<UnhandledExceptionsHandlerHostedService>();
            services.AddSingleton<ITelegramClient, TelegramClient>();
            services.AddSingleton<EmailMapper>();
            services.AddSingleton<UnhandledExceptionFilterMapper>();
            services.AddSingleton<PermittedAdminIpMapper>();

            services.AddAuthorization (options => {
                options.AddPolicy("ReqiredLoginThroughIp", policy => policy.Requirements.Add(new WhiteListIpRequirement()));
            });

            var telegramPolicy = Policy.Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(response => (int)response.StatusCode == 429)
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(30),
                });

            services.AddHttpClient();
            services.AddHttpClient("telegram")
                .AddPolicyHandler(telegramPolicy);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";//"ClientApp/dist/ClientApp";
            });

            ConfigureOptions(services);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault(),
                });
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            ConfigureImageStorage(app);

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

        protected override void AddHostedServices(IServiceCollection services)
        {
            base.AddHostedServices(services);

            services.AddSingleton<IHostedService, CreateDeviceLogsService>(serviceProvider => serviceProvider.GetService<CreateDeviceLogsService>());
            services.AddHostedService<ClearDeviceLogsService>();
            services.AddHostedService<ClearCameraLogsService>();
            services.AddSingleton<IHostedService, UnhandledExceptionsHandlerHostedService>(serviceProvider => serviceProvider.GetService<UnhandledExceptionsHandlerHostedService>());
            services.AddHostedService<UpdateDevicesSshPasswordService>();
        }

        private void ConfigureOptions(IServiceCollection services)
        {
            services.Configure<TelegramOptions>(Configuration.GetSection("Telegram"));
            services.Configure<PhysicalFileStorageOptions>(Configuration.GetSection("PhysicalFileStorage"));
            services.Configure<MobileOptions>(Configuration.GetSection("Mobile"));
            services.Configure<GoogleCloudStorageOptions>(Configuration.GetSection("GoogleCloudStorage"));
            services.Configure<CleanupLogsOptions>(Configuration.GetSection("CleanupLogs"));
            services.Configure<EmailCredentialsOptions>(Configuration.GetSection("EmailCredentials"));
            services.Configure<DemoFeatureOptions>(Configuration.GetSection("DemoFeature"));
            services.Configure<DownloadLimitsOptions>(Configuration.GetSection("DownloadLimits"));
        }

        private void ConfigureImageStorage(IApplicationBuilder app)
        {
            // File system storage for camera images
            if (Configuration.GetValue<bool>("DisableStaticFiles"))
            {
                app.UseStaticFiles();
            }
            else
            {
                var fileOptions = Configuration.GetSection("PhysicalFileStorage").Get<PhysicalFileStorageOptions>();

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider =
                        new PhysicalFileProvider(fileOptions.Root),
                    RequestPath = fileOptions.RequestPath
                });
            }
        }
    }
}
