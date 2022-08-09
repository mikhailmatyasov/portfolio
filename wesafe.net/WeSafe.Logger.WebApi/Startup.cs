using AutoMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WeSafe.Logger.Abstraction.Services;
using WeSafe.Logger.MongoDbStorage;
using WeSafe.Logger.MongoDbStorage.Models;
using WeSafe.Logger.WebApi.Services;
using WeSafe.Logger.WebApi.Services.Abstract;
using WeSafe.Logger.WebApi.Validators.LogValidators;
using WeSafe.Web.Common;
using WeSafe.Web.Common.Authentication;
using static WeSafe.Dashboard.WebApi.Proto.CameraGrpc;
using static WeSafe.Dashboard.WebApi.Proto.DeviceGrpc;

namespace WeSafe.Logger.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddFluentValidation(configuration =>
                    configuration.RegisterValidatorsFromAssemblyContaining(typeof(AddLogsCommandValidator)));

            services.AddMediatR(typeof(Startup));
            services.AddAutoMapper(typeof(Startup));
            services.AddCommon();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    };
                });

            services.AddAuthorization();

            services.AddApiVersioning(
                options => options.AssumeDefaultVersionWhenUnspecified = true);

            services.AddGrpcClient<DeviceGrpcClient>(o =>
            {
                o.Address = new System.Uri(Configuration["Services:GrpcDashboardAddress"]);
            });

            services.AddGrpcClient<CameraGrpcClient>(o =>
            {
                o.Address = new System.Uri(Configuration["Services:GrpcDashboardAddress"]);
            });

            services.AddMongoLogStorage(Configuration.GetSection(MongoLogConfiguration.ConfigurationPosition)
                                                     .Get<MongoLogConfiguration>());

            services.AddScoped<ICameraService, CameraService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IDeviceLogMapper, CameraNameLogMapper>();
            services.AddScoped<IDeviceLogMapper, DeviceNameLogMapper>();
            services.AddSingleton<IDeviceLogFilter, DeviceLogFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCommon();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
