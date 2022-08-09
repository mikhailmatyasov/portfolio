using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using FluentValidation.AspNetCore;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WeSafe.Bus.Components.StateMachines;
using WeSafe.Bus.Components.States;
using WeSafe.Bus.Contracts.User;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories;
using WeSafe.Dashboard.WebApi.Consumers;
using WeSafe.Dashboard.WebApi.Grpc;
using WeSafe.Dashboard.WebApi.Validators.Camera;
using WeSafe.Web.Common;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Dashboard.WebApi
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
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddControllers()
                    .AddNewtonsoftJson()
                    .AddFluentValidation(configuration =>
                    {
                        configuration.RegisterValidatorsFromAssemblyContaining(
                            typeof(GetCameraByIdCommandValidator));

                        configuration.AutomaticValidationEnabled = false;
                    });

            services.AddCommon();
            services.AddMediatR(typeof(Startup));

            services.AddAutoMapper(typeof(Startup));

            AddDbContext(services);

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

            // [EM]: Need uncomment to use Identity Server Authentication validation.
            //.AddJwtBearer(options =>
            //{
            //    options.Authority = "http://localhost:5000";
            //    options.RequireHttpsMetadata = false;

            //    options.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        ValidateAudience = false
            //    };
            //});

            services.AddAuthorization();

            services.AddApiVersioning(
                options => options.AssumeDefaultVersionWhenUnspecified = true);

            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<RegisterStateMachine, RegisterState>()
                 .MongoDbRepository(r =>
                 {
                     var settings = Configuration.GetSection("MongoDb");

                     r.Connection = settings["Connection"];
                     r.DatabaseName = settings["DatabaseName"];
                     r.CollectionName = settings["CollectionName"];
                 });

                x.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(Configuration.GetConnectionString("RabbitMQ"));
                    configurator.ConfigureEndpoints(context);
                });

                x.AddConsumer<AttachDeviceConsumer>();
                x.AddConsumer<CreateDeviceOwnerConsumer>();
                x.AddConsumer<UpdateDeviceTypeConsumer>();

                x.AddRequestClient<ICreateUserValidationContract>();
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

            app.UseCommon();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GrpcDeviceService>();
                endpoints.MapGrpcService<GrpcCameraService>();
                endpoints.MapControllers();
            });
        }

        protected virtual void AddDbContext(IServiceCollection serviceCollection)
        {
            var connectionString = Configuration.GetConnectionString("DB_CONNECTION_STRING");

            serviceCollection.AddEntityFrameworkNpgsql()
                             .AddDbContext<WeSafeDbContext>(options => options.UseNpgsql(connectionString))
                             .AddUnitOfWork<WeSafeDbContext>()
                             .AddCustomRepository<Device, DeviceRepository>()
                             .AddCustomRepository<ClientSubscriber, SubscriberRepository>()
                             .AddCustomRepository<Client, ClientRepository>()
                             .AddCustomRepository<Camera, CameraRepository>();
        }
    }
}