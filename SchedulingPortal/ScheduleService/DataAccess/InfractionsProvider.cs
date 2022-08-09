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
    public class InfractionsProvider : EntityProviderBase, IInfractionsProvider
    {
        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<InfractionsProvider> _logger;
        private readonly proxySoapClient _proxyClient;

        public InfractionsProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            ILogger<InfractionsProvider> logger,
            proxySoapClient proxyClient)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _logger = logger;
            _proxyClient = proxyClient;
        }

        public async Task<IEnumerable<TournamentInfraction>> GetTournamentInfractionsAsync(
            string organizationId, string tournamentId, IEnumerable<string> flightIds)
        {
            var taskList = new List<Task<IEnumerable<TournamentInfraction>>>();
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
                nameof(_proxyClient.GetTournamentFlightInfractionsAsync));
            var infractions = await Task.WhenAll(taskList.ToArray());
            performanceMeter.Stop();

            return infractions.SelectMany(x => x);
        }

        private async Task<IEnumerable<TournamentInfraction>> ProcessRequestAsync(string organizationId, string tournamentId, string flightId)
        {
            string token = _tokenProvider.GenerateAffinityApiToken();
            var request = new GetTournamentFlightInfractionsRequest
            {
                clientToken = token,
                leagueID = organizationId,
                tournamentKey = tournamentId,
                flightKey = flightId,
            };

            GetTournamentFlightInfractionsResponse response = await _proxyClient.GetTournamentFlightInfractionsAsync(request);

            return ProcessArrayResponse(
                nameof(_proxyClient.GetTournamentFlightInfractionsAsync),
                response,
                response?.seriousInfractions);
        }
    }
}
