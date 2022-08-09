using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.DataAccess.TokenProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class FlightStandingsProvider : EntityProviderBase, IFlightStandingsProvider
    {
        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<FlightStandingsProvider> _logger;
        private readonly proxySoapClient _proxyClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightStandingsProvider"/> class.
        /// </summary>
        /// <param name="tokenProvider">Token provider for getting affinity token.</param>
        /// <param name="cacheManager">Cache provider.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="proxyClient">Proxy client for getting required data.</param>
        public FlightStandingsProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            ILogger<FlightStandingsProvider> logger,
            proxySoapClient proxyClient)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _logger = logger;
            _proxyClient = proxyClient;
        }

        public async Task<IEnumerable<TournamentFlightStandings>> GetFlightStandingsListAsync(
            string organizationId,
            string tournamentId,
            IEnumerable<KeyValuePair<string, DateTime>> flightIds)
        {
            var taskList = new List<Task<TournamentFlightStandings>>();
            foreach (KeyValuePair<string, DateTime> flightInfo in flightIds)
            {
                var itemGetTask = _cacheManager.GetOrUpdateWhenExpiredAsync(
                    flightInfo.Key,
                    flightInfo.Value,
                    async () => await ProcessRequestAsync(organizationId, tournamentId, flightInfo.Key));
                taskList.Add(itemGetTask);
            }

            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetTournamentFlightStandingsAsync));
            var standings = await Task.WhenAll(taskList.ToArray());
            performanceMeter.Stop();

            return standings.Where(x => x != null);
        }

        public async Task<TournamentFlightStandings> GetFlightStandingsAsync(
            string organizationId,
            string tournamentId,
            string flightId,
            DateTime updated)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetTournamentFlightStandingsAsync));
            return await _cacheManager.GetOrUpdateWhenExpiredAsync(
                flightId,
                updated,
                async () => await ProcessRequestAsync(organizationId, tournamentId, flightId));
        }

        private async Task<TournamentFlightStandings> ProcessRequestAsync(string organizationId, string tournamentId, string flightId)
        {
            string token = _tokenProvider.GenerateAffinityApiToken();
            var request = new GetTournamentFlightStandingsRequest
            {
                clientToken = token,
                leagueID = organizationId,
                tournamentKey = tournamentId,
                flightKey = flightId,
            };
            GetTournamentFlightStandingsResponse response = await _proxyClient.GetTournamentFlightStandingsAsync(request);

            return ProcessResponse(
                nameof(_proxyClient.GetTournamentFlightStandingsAsync),
                response,
                response?.standings);
        }
    }
}
