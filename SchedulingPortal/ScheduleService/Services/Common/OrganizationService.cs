using Microsoft.Extensions.Logging;
using ProxyReference;
using ScheduleService.DataAccess;
using ScheduleService.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.Common
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationsProvider _organizationsProvider;
        private readonly ILogger<OrganizationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationService"/> class.
        /// </summary>
        /// <param name="organizationsProvider">Organizations data provider.</param>
        /// <param name="logger">Logger provider.</param>
        public OrganizationService(
            IOrganizationsProvider organizationsProvider,
            ILogger<OrganizationService> logger)
        {
            _organizationsProvider = organizationsProvider;
            _logger = logger;
        }

        /// <summary>
        /// Gets organization logo url by organizationId.
        /// </summary>
        /// <param name="organizationId">Key of the organization.</param>
        /// <returns>Organization logo url.</returns>
        public async Task<string> GetOrganizationLogoUrlAsync(string organizationId)
        {
            try
            {
                var organizations = await _organizationsProvider.GetOrganizationsAsync();

                Organization organization = organizations.FirstOrDefault(x => x.RecordKey == organizationId);

                return organization?.LogoURL ?? throw new AffinityNotFoundException($"Organization with id {organizationId} is not found.");
            }
            catch (AppBaseException e)
            {
                // ignore, logo is not important part of the response
                _logger.LogError(e, $"{nameof(GetOrganizationLogoUrlAsync)} failed");
            }

            return string.Empty;
        }
    }
}
