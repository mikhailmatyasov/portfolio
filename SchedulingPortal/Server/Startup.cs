using AutoMapper;
using Common.Cache;
using Common.Measurement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProxyReference;
using ScheduleService.DataAccess;
using ScheduleService.DataAccess.TokenProvider;
using ScheduleService.Mapping;
using ScheduleService.Services.ClubInfo;
using ScheduleService.Services.ClubStandings;
using ScheduleService.Services.Common;
using ScheduleService.Services.FieldClosures;
using ScheduleService.Services.Login;
using ScheduleService.Services.RedCards;
using ScheduleService.Services.Schedules;
using ScheduleService.Services.Standings;
using ScheduleService.Services.Stats;
using ScheduleService.Services.TeamInfo;
using ScheduleService.Services.Tournament;
using ScheduleService.Services.VenueDetails;
using ScheduleService.Services.Venues;
using ScheduleService.Services.WildCards;
using Serilog;
using Server.Configuration.AppSettings;
using Server.Configuration.Cache;
using Server.Filters;
using System;
using System.Net.Http;

namespace Server
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
            services.AddControllers(options =>
            {
                options.Filters.Add<ApiExceptionFilter>();
            });
#if DEBUG
            services.AddCors();
#endif
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Server", Version = "v1" });
                c.CustomSchemaIds(type => type.ToString());
            });

            services.AddHttpClient<ILoginService, LoginService>(c =>
            {
                c.BaseAddress = new Uri(Configuration["AffinityLogin:BaseUrl"]);
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
            });
            PerformanceMeter.SetPerformanceMeasurement(Configuration.GetSection("PerformanceMeasurement").Get<bool>());

            // configure logging
            services.AddSingleton(Log.Logger);

            // configure mapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new TournamentMappingProfile());
                mc.AddProfile(new VenuesMappingProfile());
                mc.AddProfile(new VenueDetailsMappingProfile());
                mc.AddProfile(new StandingsMappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // configure cache
            services.Configure<CacheExpirationOptions>(Configuration.GetSection(CacheExpirationOptions.SectionName));
            services.AddSingleton(CacheStorage.GetInstance());
            services.AddSingleton<ICacheManagerConfiguration, CacheManagerConfiguration>();
            services.AddSingleton<ICacheManager, CacheManager>();

            // configure data access
            services.AddScoped(_ => new proxySoapClient(proxySoapClient.EndpointConfiguration.proxySoap));
            var affinityTokenConfiguration = Configuration.GetSection("AffinityToken").Get<AffinityTokenConfiguration>();
            services.AddSingleton<IAffinityTokenProvider, AffinityTokenProvider>(_ =>
                new AffinityTokenProvider(affinityTokenConfiguration.ModuleId, affinityTokenConfiguration.SecretKey));
            services.AddScoped<ITournamentProvider, TournamentProvider>();
            services.AddScoped<IFlightGamesProvider, FlightGamesProvider>();
            services.AddScoped<ITournamentVenuesProvider, TournamentVenuesProvider>();
            services.AddScoped<ITournamentVenueDetailsProvider, TournamentVenueDetailsProvider>();
            services.AddScoped<IFlightStandingsProvider, FlightStandingsProvider>();
            services.AddScoped<ITournamentTeamProvider, TournamentTeamProvider>();
            services.AddScoped<IOrganizationsProvider, OrganizationsProvider>();
            services.AddScoped<IOrganizationGendersProvider, OrganizationGendersProvider>();
            services.AddScoped<ILeaguesAndClubsProvider, LeaguesAndClubsProvider>();
            services.AddScoped<IInfractionsProvider, InfractionsProvider>();
            services.AddScoped<ITournamentTopScorerProvider, TournamentTopScorerProvider>();

            // configure services
            services.AddScoped<ITournamentService, TournamentService>();
            services.AddScoped<IVenuesService, VenuesService>();
            services.AddScoped<IVenueDetailsService, VenueDetailsService>();
            services.AddScoped<ISchedulesService, SchedulesService>();
            services.AddScoped<IFieldClosuresService, FieldClosuresService>();
            services.AddScoped<IClubStandingsService, ClubStandingsService>();
            services.AddScoped<IClubInfoService, ClubInfoService>();
            services.AddScoped<ITeamInfoService, TeamInfoService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IPageHeaderService, PageHeaderService>();
            services.AddScoped<IStandingsService, StandingsService>();
            services.AddScoped<IStatsService, StatsService>();
            services.AddScoped<IRedCardsService, RedCardsService>();
            services.AddScoped<IWildCardsService, WildCardsService>();

            WeatherConfiguration weatherConfiguration = Configuration.GetSection("WeatherConfiguration").Get<WeatherConfiguration>();
            HttpClient weatherHttpClient = new() { BaseAddress = new Uri(weatherConfiguration.Host) };
            services.AddSingleton<IWeatherTokenProvider>(_ => new AccuWeatherTokenProvider(weatherConfiguration.JwtIss, weatherConfiguration.JwtSecret));
            services.AddSingleton<IWeatherProvider>(p =>
                new AccuWeatherProvider(weatherHttpClient, p.GetService<IWeatherTokenProvider>(), p.GetService<ILogger<AccuWeatherProvider>>()));
            services.AddSingleton<IWeatherService, AccuWeatherService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
#if DEBUG
            app.UseCors(configurePolicy =>
            {
                configurePolicy.AllowAnyOrigin();
                configurePolicy.AllowAnyHeader();
                configurePolicy.AllowAnyMethod();
            });
#endif
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
