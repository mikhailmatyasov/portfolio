using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.DataAccess.TokenProvider;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class TournamentProvider : EntityProviderBase, ITournamentProvider
    {
        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly proxySoapClient _proxyClient;
        private readonly ILogger<TournamentProvider> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentProvider"/> class.
        /// </summary>
        /// <param name="tokenProvider">Token provider for getting affinity token.</param>
        /// <param name="cacheManager">Cache provider.</param>
        /// <param name="proxyClient">Proxy client for getting required data.</param>
        /// <param name="logger">Logger.</param>
        public TournamentProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            proxySoapClient proxyClient,
            ILogger<TournamentProvider> logger)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _proxyClient = proxyClient;
            _logger = logger;
        }

        /// <summary>
        /// Gets tournament data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Tournament.</returns>
        public async Task<Tournament> GetTournamentAsync(string organizationId, string tournamentId)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetTournamentAsync));
            return await _cacheManager.GetOrAddDataAsync(
                tournamentId,
                async () => await ProcessRequestAsync(organizationId, tournamentId));
        }

        private async Task<Tournament> ProcessRequestAsync(string organizationId, string tournamentId)
        {
            string token = _tokenProvider.GenerateAffinityApiToken();
            GetTournamentRequest request = new GetTournamentRequest
            {
                clientToken = token,
                leagueID = organizationId,
                tournamentKey = tournamentId,
            };
            GetTournamentResponse response = await _proxyClient.GetTournamentAsync(request);
            return ProcessResponse(
                nameof(_proxyClient.GetTournamentAsync),
                response,
                response?.tournament);
        }
    }
}
