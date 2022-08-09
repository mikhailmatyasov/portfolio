using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess.TokenProvider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class TournamentVenuesProvider : PagedEntityProvider, ITournamentVenuesProvider
    {
        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly proxySoapClient _proxyClient;
        private readonly ILogger<TournamentVenuesProvider> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentVenuesProvider"/> class.
        /// </summary>
        /// <param name="tokenProvider">Token provider for getting affinity token.</param>
        /// <param name="cacheManager">Cache provider.</param>
        /// <param name="proxyClient">Proxy client for getting required data.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="configuration">Application configuration properties.</param>
        public TournamentVenuesProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            proxySoapClient proxyClient,
            ILogger<TournamentVenuesProvider> logger,
            IConfiguration configuration)
            : base(configuration)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _proxyClient = proxyClient;
            _logger = logger;
        }

        /// <summary>
        /// Gets tournament venues by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Tournament venues.</returns>
        public async Task<TournamentVenues> GetTournamentVenuesAsync(string organizationId, string tournamentId)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetAllTournamentVenuesAsync));
            return await _cacheManager.GetOrAddDataAsync(
                tournamentId,
                async () => await GetTournamentVenuesListAsync(organizationId, tournamentId));
        }

        protected override int GetResponseItemsCount(PagedResponse response)
        {
            return ((GetAllTournamentVenuesResponse)response).venues?.Length ?? 0;
        }

        private async Task<TournamentVenues> GetTournamentVenuesListAsync(string organizationId, string tournamentId)
        {
            string token = _tokenProvider.GenerateAffinityApiToken();

            var request = new GetAllTournamentVenuesRequest
            {
                clientToken = token,
                leagueID = organizationId,
                tournamentKey = tournamentId,
            };

            var responses = await ProcessPagedRequest(
                request,
                async req => await _proxyClient.GetAllTournamentVenuesAsync(req));

            var venues = new List<TournamentVenue>();
            foreach (GetAllTournamentVenuesResponse response in responses)
            {
                venues.AddRange(ProcessArrayResponse(
                    nameof(_proxyClient.GetAllTournamentVenuesAsync),
                    response,
                    response?.venues));
            }

            return new TournamentVenues { TournamentKey = tournamentId, Venues = venues };
        }
    }
}
