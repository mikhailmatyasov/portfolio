
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Threading.Tasks;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Roles;
using WeSafe.Web.Core.Authentication;
using WeSafe.Web.Core.Handlers;
using WeSafe.Web.Core.Hubs;
using WeSafe.Web.Core.Mappers;

namespace WeSafe.Web.Core
{
    public class BaseStartup
    {
        #region Constructor

        public BaseStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region Properties

        public IConfiguration Configuration { get; }

        #endregion

        #region Public methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            AddDbContext(services);

            services.AddScoped<CameraSchedulerValidator>();
            services.AddScoped<ICameraService, CameraService>();

            services.AddIdentity<User, IdentityRole<string>>()
                    .AddEntityFrameworkStores<WeSafeDbContext>()
                    .AddDefaultTokenProviders();

            AddHostedServices(services);

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
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                // If the request is for our hub...
                                var path = context.HttpContext.Request.Path;

                                if (!String.IsNullOrEmpty(accessToken) &&
                                     (path.StartsWithSegments("/eventshub") || path.StartsWithSegments("/api/deviceLog/downloadLogs")))
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorsRole", policy => policy.RequireRole(UserRoles.Administrators));
                options.AddPolicy("RequireUsersRole", policy => policy.RequireRole(UserRoles.Users));
                options.AddPolicy("RequireDevicesRole", policy => policy.RequireRole("Devices"));
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddControllers().AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WeSafe API",
                    Version = "v1",
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });

            AddSingletonServices(services);
            AddScopedServices(services);
            AddTransientServices(services);

            services.AddSignalR();
            services.AddApiVersioning(
                options => options.AssumeDefaultVersionWhenUnspecified = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseGlobalExceptionHandler(env.IsDevelopment());

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(config =>
            {
                config.MapHub<EventsHub>("/eventshub");
                config.MapControllers();
            });

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeSafe API V1"); });
            }
        }

        #endregion

        #region Protected methods

        protected virtual void AddDbContext(IServiceCollection serviceCollection)
        {
            var connectionString = Configuration.GetConnectionString("DB_CONNECTION_STRING");

            serviceCollection.AddDbContext<WeSafeDbContext>(options => options.UseNpgsql(connectionString));
        }

        protected virtual void MigrateAndSeed(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WeSafeDbContext>();

                context.Database.Migrate();
                context.Seed(scope);
            }
        }

        protected virtual void AddHostedServices(IServiceCollection services)
        {

        }

        #endregion

        #region Private methods

        private void AddSingletonServices(IServiceCollection services)
        {
            services.AddSingleton<CameraMapper>();
            services.AddSingleton<UserMapper>();
            services.AddSingleton<DeviceMapper>();
            services.AddSingleton<DeviceLogMapper>();
            services.AddSingleton<DeviceLogPresentationMapper>();
            services.AddSingleton<ClientDeviceLogPresentationMapper>();
            services.AddSingleton<DeviceLogFilterMapper>();
            services.AddSingleton<ClientMapper>();
            services.AddSingleton<RtspPathMapper>();
            services.AddSingleton<CameraManufactorMapper>();
            services.AddSingleton<CameraMarkMapper>();
            services.AddSingleton<ClientSubscriberMapper>();
            services.AddSingleton<CreateDeviceLogsService>();
        }

        private void AddScopedServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IDeviceLogService, DeviceLogService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IRtspPathService, RtspPathService>();
            services.AddScoped<ICameraManufactorService, CameraManufactorService>();
            services.AddScoped<ICameraLogService, CameraLogService>();
            services.AddScoped<IClientSubscriberService, ClientSubscriberService>();
            services.AddScoped<IAuthTokenGenerator, AuthTokenGenerator>();
            services.AddScoped<ExceptionMapper>();
            services.AddScoped<IFileStorage, PhysicalFileStorage>();
            services.AddScoped<ITelegramService, TelegramService>();
            services.AddScoped<ITrafficService, TrafficService>();
            services.AddScoped<IPlateEventService, PlateEventService>();
            services.AddScoped<ILicensePlateRestrictionService, LicensePlateRestrictionService>();
            services.AddScoped<IDeviceIndicatorsService, DeviceIndicatorsService>();
            services.AddScoped<IGlobalSettingsService, GlobalSettingsService>();
            services.AddScoped<IRecognitionObjectService, RecognitionObjectService>();
            services.AddScoped<IDetectedCameraService, DetectedCameraService>();
            services.AddScoped<IDeviceMetadataService, DeviceMetadataService>();
        }

        private void AddTransientServices(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }

        #endregion
    }
}
