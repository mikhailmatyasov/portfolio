using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.DataAccess.TokenProvider;
using ScheduleService.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class OrganizationsProvider : EntityProviderBase, IOrganizationsProvider
    {
        private const string CacheKey = "GetOrganizationsKey";

        private readonly IAffinityTokenProvider _tokenProvider;
        private readonly ICacheManager _cacheManager;
        private readonly proxySoapClient _proxyClient;
        private readonly ILogger<OrganizationsProvider> _logger;

        public OrganizationsProvider(
            IAffinityTokenProvider tokenProvider,
            ICacheManager cacheManager,
            proxySoapClient proxyClient,
            ILogger<OrganizationsProvider> logger)
        {
            _tokenProvider = tokenProvider;
            _cacheManager = cacheManager;
            _proxyClient = proxyClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Organization>> GetOrganizationsAsync()
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(_proxyClient.GetAuthorizedOrganizationsAsync));
            return await _cacheManager.GetOrAddDataAsync(
            CacheKey,
            async () => await ProcessRequestAsync());
        }

        private async Task<IEnumerable<Organization>> ProcessRequestAsync()
        {
            string token = _tokenProvider.GenerateAffinityApiToken();
            var request = new GetAuthorizedOrganizationsRequest
            {
                clientToken = token,
            };

            GetAuthorizedOrganizationsResponse response = await _proxyClient.GetAuthorizedOrganizationsAsync(request);

            // we can't use base class ProcessResponse method, cause the response class is not derived form BaseResponse
            return ProcessResponse(nameof(_proxyClient.GetAuthorizedOrganizationsAsync), response);
        }

        private IEnumerable<Organization> ProcessResponse(string apiMethodName, GetAuthorizedOrganizationsResponse response)
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
                    return response.organizations ?? Array.Empty<Organization>();
                }

                throw new AffinityServerErrorException(
                    $"{apiMethodName} call returned error code {response.resultCode}.");
            }

            if (response.organizations == null)
            {
                throw new AffinityServerErrorException(
                    $"{apiMethodName} call returned null in result field!");
            }

            return response.organizations;
        }
    }
}
