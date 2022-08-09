using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.DataAccess.TokenProvider;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class TournamentTeamProvider : EntityProviderBase, ITournamentTeamProvider
    {
        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly proxySoapClient _proxyClient;
        private readonly ILogger<TournamentTeamProvider> _logger;

        public TournamentTeamProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            proxySoapClient proxyClient,
            ILogger<TournamentTeamProvider> logger)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _proxyClient = proxyClient;
            _logger = logger;
        }

        public async Task<TeamWithAdminsPlayers3> GetTournamentTeamAsync(string organizationId, string tournamentId, string teamId)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetTournamentTeamAsync));
            return await _cacheManager.GetOrAddDataAsync(
                teamId,
                async () => await ProcessRequestAsync(organizationId, tournamentId, teamId));
        }

        private async Task<TeamWithAdminsPlayers3> ProcessRequestAsync(string organizationId, string tournamentId, string teamId)
        {
            string token = _tokenProvider.GenerateAffinityApiToken();

            var request = new GetTournamentTeamRequest
            {
                clientToken = token,
                leagueID = organizationId,
                tournamentKey = tournamentId,
                teamKey = teamId,
            };

            GetTournamentTeamResponse3 response = await _proxyClient.GetTournamentTeamAsync(request);

            return ProcessResponse(
                nameof(_proxyClient.GetTournamentTeamAsync),
                response,
                response?.team);
        }
    }
}
