using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using FluentValidation.AspNetCore;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WeSafe.DAL;
using WeSafe.Event.WebApi.Consumers;
using WeSafe.Event.WebApi.Services;
using WeSafe.Event.WebApi.Services.Abstract;
using WeSafe.Web.Common;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Event.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(
                options => options.AssumeDefaultVersionWhenUnspecified = true);

            services.AddControllers()
                .AddNewtonsoftJson()
                .AddFluentValidation(configuration =>
                {
                    configuration.RegisterValidatorsFromAssemblyContaining(
                        typeof(Startup));

                    configuration.AutomaticValidationEnabled = false;
                });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddAutoMapper(typeof(Startup));
            AddDbContext(services);
            services.AddCommon();
            services.AddMediatR(typeof(Startup));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
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

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(Configuration.GetConnectionString("RabbitMQ"));
                    configurator.ConfigureEndpoints(context);
                });

                x.AddConsumer<AddEventConsumer>();
            });

            services.AddMassTransitHostedService();

            services.AddScoped<IBusCommandWrapper, BusCommandWrapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCommon();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected virtual void AddDbContext(IServiceCollection serviceCollection)
        {
            var connectionString = Configuration.GetConnectionString("DB_CONNECTION_STRING");

            serviceCollection.AddEntityFrameworkNpgsql()
                .AddDbContext<WeSafeDbContext>(options => options.UseNpgsql(connectionString))
                .AddUnitOfWork<WeSafeDbContext>();
        }
    }
}
