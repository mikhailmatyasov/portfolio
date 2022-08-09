using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using FluentValidation.AspNetCore;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using WeSafe.Authentication.WebApi.Authorization;
using WeSafe.Authentication.WebApi.Consumers;
using WeSafe.Authentication.WebApi.Services;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.Authentication.WebApi.Validators.Login;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories;
using WeSafe.Web.Common;
using WeSafe.Web.Common.Authentication;
using WeSafe.Web.Common.Authentication.Abstract;
using WeSafe.Web.Common.Exceptions.Handlers;

namespace WeSafe.Authentication.WebApi
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
            services.AddCommon();
            services.AddScoped<IUserManager, WeSafeUserManager>();

            AddDbContext(services);

            services.AddIdentity<User, IdentityRole<string>>()
                    .AddEntityFrameworkStores<WeSafeDbContext>()
                    .AddDefaultTokenProviders();

            // [EM]: Uncomment this line to include Identity Server 4. 
            //AddIdentityServer(services);


            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.RequireUniqueEmail = false;
            });

            services.AddSingleton<IAuthTokenGenerator, AuthTokenGenerator>();

            services.AddControllers()
                    .AddNewtonsoftJson()
                    .AddFluentValidation(configuration =>
                    {
                        configuration.RegisterValidatorsFromAssemblyContaining(typeof(LoginCommandValidator));
                        configuration.AutomaticValidationEnabled = false;
                    });

            services.AddMediatR(typeof(Startup));
            services.AddAutoMapper(typeof(Startup));

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(Configuration.GetConnectionString("RabbitMQ"));
                    configurator.ConfigureEndpoints(context);
                });

                x.AddConsumer<CreateUserConsumer>();
                x.AddConsumer<CreateUserValidatorConsumer>();
            });

            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            }

            app.UseGlobalExceptionHandler(false);

            app.UseCommon();

            app.UseRouting();
            // [EM]: Uncomment this line to activate Identity Server 4.
            //app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        protected virtual void AddDbContext(IServiceCollection serviceCollection)
        {
            var connectionString = Configuration.GetConnectionString("DB_CONNECTION_STRING");

            serviceCollection.AddDbContext<WeSafeDbContext>(options => options.UseNpgsql(connectionString))
                             .AddUnitOfWork<WeSafeDbContext>()
                             .AddCustomRepository<Device, DeviceRepository>()
                             .AddCustomRepository<MobileUser, MobileUserRepository>()
                             .AddCustomRepository<ClientSubscriber, SubscriberRepository>();
        }

        private void AddIdentityServer(IServiceCollection services)
        {
            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(AuthorizationConfig.IdentityResources)
                .AddInMemoryClients(AuthorizationConfig.Clients)
                .AddAspNetIdentity<User>()
                .AddInMemoryApiScopes(AuthorizationConfig.ApiScopes)
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidatorService>()
                .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential();

            services.AddTransient<IResourceOwnerPassword, WebResourceOwnerPassword>();
            services.AddTransient<IResourceOwnerPassword, MobileResourceOwnerPassword>();
            services.AddTransient<IResourceOwnerPassword, DeviceResourceOwnerPassword>();
            services.AddTransient<IResourceOwnerPasswordFactory, ResourceOwnerPasswordFactory>();

            services.AddTransient<ICustomProfileService, WebCustomProfileService>();
            services.AddTransient<ICustomProfileService, MobileCustomProfileService>();
            services.AddTransient<ICustomProfileService, DeviceCustomProfileService>();
            services.AddTransient<ICustomProfileServiceFactory, CustomProfileServiceFactory>();
        }
    }
}