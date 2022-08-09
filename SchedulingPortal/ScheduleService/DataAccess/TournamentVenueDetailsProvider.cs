using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.DataAccess.TokenProvider;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class TournamentVenueDetailsProvider : EntityProviderBase, ITournamentVenueDetailsProvider
    {
        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<TournamentVenueDetailsProvider> _logger;
        private readonly proxySoapClient _proxyClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentVenueDetailsProvider"/> class.
        /// </summary>
        /// <param name="tokenProvider">Token provider for getting affinity token.</param>
        /// <param name="cacheManager">Cache provider.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="proxyClient">Proxy client for getting required data.</param>
        public TournamentVenueDetailsProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            ILogger<TournamentVenueDetailsProvider> logger,
            proxySoapClient proxyClient)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _logger = logger;
            _proxyClient = proxyClient;
        }

        /// <summary>
        /// Gets venue details data by organizationId and venueId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="venueId">Key of the venue.</param>
        /// <returns>Tournament.</returns>
        public async Task<TournamentVenue> GetVenueDetailsAsync(string organizationId, string venueId)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetVenueAsync));
            return await _cacheManager.GetOrAddDataAsync(
                venueId,
                async () => await ProcessRequestAsync(organizationId, venueId));
        }

        private async Task<TournamentVenue> ProcessRequestAsync(string organizationId, string venueId)
        {
            string token = _tokenProvider.GenerateAffinityApiToken();
            GetVenueRequest request = new GetVenueRequest
            {
                clientToken = token,
                leagueID = organizationId,
                venueKey = venueId,
            };
            GetVenueResponse response = await _proxyClient.GetVenueAsync(request);
            return ProcessResponse(
                nameof(_proxyClient.GetTournamentAsync),
                response,
                response?.venue);
        }
    }
}
