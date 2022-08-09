using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess.TokenProvider;
using ScheduleService.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class LeaguesAndClubsProvider : EntityProviderBase, ILeaguesAndClubsProvider
    {
        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<LeaguesAndClubsProvider> _logger;
        private readonly proxySoapClient _proxyClient;
        private readonly int _maxPageSize = 500;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaguesAndClubsProvider"/> class.
        /// </summary>
        /// <param name="tokenProvider">Token provider for getting affinity token.</param>
        /// <param name="cacheManager">Cache provider.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="proxyClient">Proxy client for getting required data.</param>
        public LeaguesAndClubsProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            ILogger<LeaguesAndClubsProvider> logger,
            proxySoapClient proxyClient)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _logger = logger;
            _proxyClient = proxyClient;
        }

        /// <summary>
        /// Gets organization leagues with clubs by organizationId.
        /// </summary>
        /// <param name="organizationId">Key of the organization.</param>
        /// <returns>Organization leagues with clubs.</returns>
        public async Task<OrganizationLeaguesWithClubs> GetOrganizationLeaguesWithClubsAsync(string organizationId)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetAuthorizedLeaguesAndClubsAsync));
            return await _cacheManager.GetOrAddDataAsync(
                organizationId,
                async () => await ProcessRequestAsync(organizationId));
        }

        private async Task<OrganizationLeaguesWithClubs> ProcessRequestAsync(string organizationId)
        {
            string token = _tokenProvider.GenerateAffinityApiToken();
            GetAuthorizedLeaguesAndClubsRequest request = new()
            {
                clientToken = token,
                organizationID = organizationId,
            };

            // we can't use base class ProcessPagedRequest method, cause the GetAuthorizedLeaguesAndClubsRequest class is not derived form PagedRequest
            IEnumerable<GetAuthorizedLeaguesAndClubsResponse> responses =
                await ProcessPagedRequest(request, r => _proxyClient.GetAuthorizedLeaguesAndClubsAsync(r));

            // we can't use base class ProcessResponse method, cause the response class is not derived form BaseResponse
            var leagueWithClubs = ProcessResponse(nameof(_proxyClient.GetAuthorizedLeaguesAndClubsAsync), responses);
            return new OrganizationLeaguesWithClubs
            {
                OrganizationId = organizationId,
                LeagueWithClubs = leagueWithClubs,
            };
        }

        private async Task<IEnumerable<GetAuthorizedLeaguesAndClubsResponse>> ProcessPagedRequest(
            GetAuthorizedLeaguesAndClubsRequest request,
            Func<GetAuthorizedLeaguesAndClubsRequest, Task<GetAuthorizedLeaguesAndClubsResponse>> apiMethodFunc)
        {
            request.pageSize = _maxPageSize;

            var responses = new List<GetAuthorizedLeaguesAndClubsResponse>();

            int responseItemsCount;
            var pageIndex = 0;
            do
            {
                request.page = pageIndex++;
                GetAuthorizedLeaguesAndClubsResponse response = await apiMethodFunc.Invoke(request);

                responses.Add(response);
                responseItemsCount = response?.leagues?.Length ?? 0;
            }
            while (responseItemsCount >= _maxPageSize);

            return responses;
        }

        private IEnumerable<LeagueWithClubs> ProcessResponse(
            string apiMethodName,
            IEnumerable<GetAuthorizedLeaguesAndClubsResponse> responses)
        {
            List<LeagueWithClubs> result = new();
            foreach (GetAuthorizedLeaguesAndClubsResponse response in responses)
            {
                if (response == null)
                {
                    throw new AffinityServerErrorException($"{apiMethodName} call returned null.");
                }

                if (response.resultCode != enuResultCodes.Success)
                {
                    if (EntityNotFoundCodes.Contains(response.resultCode))
                    {
                        throw new AffinityNotFoundException(
                            $"{apiMethodName} not found due to error code {response.resultCode}.");
                    }

                    if (NoMatchingItemCodes.Contains(response.resultCode))
                    {
                        return response.leagues ?? Array.Empty<LeagueWithClubs>();
                    }

                    throw new AffinityServerErrorException(
                        $"{apiMethodName} call returned error code {response.resultCode}.");
                }

                if (response.leagues == null)
                {
                    throw new AffinityServerErrorException(
                        $"{apiMethodName} call returned null in result field!");
                }

                result.AddRange(response.leagues);
            }

            return result;
        }
    }
}
