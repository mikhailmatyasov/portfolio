using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess.TokenProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class FlightGamesProvider : PagedEntityProvider, IFlightGamesProvider
    {
        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly proxySoapClient _proxyClient;
        private readonly ILogger<FlightGamesProvider> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightGamesProvider"/> class.
        /// </summary>
        /// <param name="tokenProvider">Token provider for getting affinity token.</param>
        /// <param name="cacheManager">Cache provider.</param>
        /// <param name="proxyClient">Proxy client for getting required data.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public FlightGamesProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            proxySoapClient proxyClient,
            ILogger<FlightGamesProvider> logger,
            IConfiguration configuration)
            : base(configuration)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _proxyClient = proxyClient;
            _logger = logger;
        }

        /// <summary>
        /// Gets flight games list by organizationId, tournamentId and flightIds.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="flightIds">List of the flights with flight updated info.</param>
        /// <returns>Flight games list.</returns>
        public async Task<IEnumerable<FlightGames>> GetFlightGamesListAsync(
            string organizationId,
            string tournamentId,
            IEnumerable<KeyValuePair<string, DateTime>> flightIds)
        {
            var taskList = new List<Task<FlightGames>>();
            foreach (KeyValuePair<string, DateTime> flightData in flightIds)
            {
                var itemGetTask = _cacheManager.GetOrUpdateWhenExpiredAsync(
                    flightData.Key,
                    flightData.Value,
                    async () => await ProcessRequestAsync(organizationId, tournamentId, flightData.Key));
                taskList.Add(itemGetTask);
            }

            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetTournamentFlightGamesAsync));
            return await Task.WhenAll(taskList.ToArray());
        }

        /// <summary>
        /// Gets flight games list by organizationId, tournamentId and flightId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="flightId">Key of the flight.</param>
        /// <param name="updated">Flight last updated.</param>
        /// <returns>Flight games list.</returns>
        public async Task<FlightGames> GetFlightGamesAsync(string organizationId, string tournamentId, string flightId, DateTime updated)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetTournamentFlightGamesAsync));
            return await _cacheManager.GetOrUpdateWhenExpiredAsync(
                flightId,
                updated,
                async () => await ProcessRequestAsync(organizationId, tournamentId, flightId));
        }

        protected override int GetResponseItemsCount(PagedResponse response)
        {
            return ((GetTournamentFlightGamesResponse)response).games?.Length ?? 0;
        }

        private async Task<FlightGames> ProcessRequestAsync(
            string organizationId,
            string tournamentId,
            string flightId)
        {
            string token = _tokenProvider.GenerateAffinityApiToken();

            var request = new GetTournamentFlightGamesRequest
            {
                clientToken = token,
                leagueID = organizationId,
                tournamentKey = tournamentId,
                flightKey = flightId,
            };

            var responses = await ProcessPagedRequest(
                request,
                async req => await _proxyClient.GetTournamentFlightGamesAsync(req));

            var games = new List<TournamentGame>();

            foreach (GetTournamentFlightGamesResponse response in responses)
            {
                games.AddRange(ProcessArrayResponse(
                    nameof(_proxyClient.GetTournamentFlightGamesAsync),
                    response,
                    response?.games ?? Array.Empty<TournamentGame>()));
            }

            return new FlightGames
            {
                FlightKey = flightId,
                Games = games.GroupBy(g => g.RecordKey).Select(h => h.First()).ToList(),
            };
        }
    }
}
