using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess.TokenProvider;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class OrganizationGendersProvider : EntityProviderBase, IOrganizationGendersProvider
    {
        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<OrganizationGendersProvider> _logger;
        private readonly proxySoapClient _proxyClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationGendersProvider"/> class.
        /// </summary>
        /// <param name="tokenProvider">Token provider for getting affinity token.</param>
        /// <param name="cacheManager">Cache provider.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="proxyClient">Proxy client for getting required data.</param>
        public OrganizationGendersProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            ILogger<OrganizationGendersProvider> logger,
            proxySoapClient proxyClient)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _logger = logger;
            _proxyClient = proxyClient;
        }

        /// <summary>
        /// Gets organization genders by organizationId.
        /// </summary>
        /// <param name="organizationId">Key of the organization.</param>
        /// <returns>OrganizationGenders.</returns>
        public async Task<OrganizationGenders> GetOrganizationGendersAsync(string organizationId)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetGendersAsync));
            return await _cacheManager.GetOrAddDataAsync(
                organizationId,
                async () => await ProcessRequestAsync(organizationId));
        }

        private async Task<OrganizationGenders> ProcessRequestAsync(string organizationId)
        {
            string token = _tokenProvider.GenerateAffinityApiToken();
            GetGendersRequest request = new()
            {
                clientToken = token,
                leagueID = organizationId,
            };
            GetGendersResponse response = await _proxyClient.GetGendersAsync(request);
            var genders = ProcessArrayResponse(
                nameof(_proxyClient.GetGendersAsync),
                response,
                response?.genders);
            return new OrganizationGenders
            {
                OrganizationId = organizationId,
                Genders = genders,
            };
        }
    }
}
