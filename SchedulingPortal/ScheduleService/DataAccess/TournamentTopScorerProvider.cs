using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.DataAccess.TokenProvider;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class TournamentTopScorerProvider : EntityProviderBase, ITournamentTopScorerProvider
    {
        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<TournamentTopScorerProvider> _logger;
        private readonly proxySoapClient _proxyClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentTopScorerProvider"/> class.
        /// </summary>
        /// <param name="tokenProvider">Token provider for getting affinity token.</param>
        /// <param name="cacheManager">Cache provider.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="proxyClient">Proxy client for getting required data.</param>
        public TournamentTopScorerProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            ILogger<TournamentTopScorerProvider> logger,
            proxySoapClient proxyClient)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _logger = logger;
            _proxyClient = proxyClient;
        }

        /// <summary>
        /// Gets players data w/ goals data by organizationId, tournamentId and flightIds.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="flightIds">Keys of the Flights/AgeGroups.</param>
        /// <returns>Collection of TournamentTopScorer.</returns>
        public async Task<IEnumerable<TournamentTopScorer>> GetPlayersWithScoresAsync(
            string organizationId,
            string tournamentId,
            IEnumerable<string> flightIds)
        {
            var taskList = new List<Task<IEnumerable<TournamentTopScorer>>>();
            foreach (string flightId in flightIds)
            {
                var itemGetTask = _cacheManager.GetOrAddDataAsync(
                    flightId,
                    async () => await ProcessRequestAsync(organizationId, tournamentId, flightId));
                taskList.Add(itemGetTask);
            }

            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetTournamentFlightTopScorersAsync));
            var result = await Task.WhenAll(taskList.ToArray());
            performanceMeter.Stop();
            var data = result.SelectMany(x => x);
            return data;
        }

        private async Task<IEnumerable<TournamentTopScorer>> ProcessRequestAsync(string organizationId, string tournamentId, string flightId)
        {
            string token = _tokenProvider.GenerateAffinityApiToken();
            GetTournamentFlightTopScorersRequest request = new GetTournamentFlightTopScorersRequest
            {
                clientToken = token,
                leagueID = organizationId,
                tournamentKey = tournamentId,
                flightKey = flightId,
            };
            GetTournamentFlightTopScorersResponse response = await _proxyClient.GetTournamentFlightTopScorersAsync(request);
            return ProcessArrayResponse(
                nameof(_proxyClient.GetTournamentFlightTopScorersAsync),
                response,
                response?.scorers);
        }
    }
}
