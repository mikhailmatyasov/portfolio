using Common.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Model.Models;
using ProxyReference;
using ScheduleService.CacheEntities;
using Server.Configuration.AppSettings;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Server.Configuration.Cache
{
    public class CacheManagerConfiguration : ICacheManagerConfiguration
    {
        public CacheManagerConfiguration(IOptions<CacheExpirationOptions> cacheExpirationOptions)
        {
            var options = cacheExpirationOptions.Value;
            DefaultMemoryCacheEntryOptions = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.MediumDataTimeoutMinutes) };
            var cacheOptionsDictionaryBuilder = ImmutableDictionary.CreateBuilder<Type, MemoryCacheEntryOptions>();

            // Static cache.
            MemoryCacheEntryOptions staticCache = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.StaticDataTimeoutMinutes) };
            cacheOptionsDictionaryBuilder.Add(typeof(OrganizationGenders), staticCache);
            cacheOptionsDictionaryBuilder.Add(typeof(IEnumerable<Organization>), staticCache);
            cacheOptionsDictionaryBuilder.Add(typeof(OrganizationLeaguesWithClubs), staticCache);
            cacheOptionsDictionaryBuilder.Add(typeof(TournamentVenue), staticCache);

            // Medium cache.
            MemoryCacheEntryOptions mediumCache = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.MediumDataTimeoutMinutes) };
            cacheOptionsDictionaryBuilder.Add(typeof(TournamentVenues), mediumCache);
            cacheOptionsDictionaryBuilder.Add(typeof(TeamWithAdminsPlayers3), mediumCache);
            cacheOptionsDictionaryBuilder.Add(typeof(IEnumerable<TournamentInfraction>), mediumCache);
            cacheOptionsDictionaryBuilder.Add(typeof(IEnumerable<TournamentTopScorer>), mediumCache);

            // Fluid cache.
            MemoryCacheEntryOptions fluidCache = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.FluidDataTimeoutMinutes) };
            cacheOptionsDictionaryBuilder.Add(typeof(Tournament), fluidCache);

            // Constant cache. Refreshed only when "updated" changed.
            MemoryCacheEntryOptions constantCache = new();
            cacheOptionsDictionaryBuilder.Add(typeof(FlightGames), constantCache);
            cacheOptionsDictionaryBuilder.Add(typeof(TournamentFlightStandings), constantCache);

            // Weather cache.
            MemoryCacheEntryOptions weatherCache = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(options.WeatherTimeoutInHours) };
            cacheOptionsDictionaryBuilder.Add(typeof(WeatherData), weatherCache);

            CustomCacheEntryOptions = cacheOptionsDictionaryBuilder.ToImmutable();
        }

        public MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions { get; }

        public IDictionary<Type, MemoryCacheEntryOptions> CustomCacheEntryOptions { get; }
    }
}
