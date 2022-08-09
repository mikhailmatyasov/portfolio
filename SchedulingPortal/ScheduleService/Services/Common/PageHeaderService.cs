using AutoMapper;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.Common;
using ScheduleService.DataAccess;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.Common
{
    public class PageHeaderService : IPageHeaderService
    {
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IOrganizationService _organizationService;
        private readonly ILogger<PageHeaderService> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageHeaderService"/> class.
        /// </summary>
        /// <param name="tournamentProvider">Tournament data provider.</param>
        /// <param name="mapper">Mapper to convert into DTOs.</param>
        /// <param name="organizationService">Organization service.</param>
        /// <param name="logger">Logger.</param>
        public PageHeaderService(
            ITournamentProvider tournamentProvider,
            IMapper mapper,
            IOrganizationService organizationService,
            ILogger<PageHeaderService> logger)
        {
            _tournamentProvider = tournamentProvider;
            _mapper = mapper;
            _organizationService = organizationService;
            _logger = logger;
        }

        /// <summary>
        /// Gets data for subpage header by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Subpage header data.</returns>
        public async Task<PageHeaderDto> GetPageHeaderDataAsync(string organizationId, string tournamentId)
        {
            using var tournamentPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.PageHeaderMeasurement,
                nameof(_tournamentProvider.GetTournamentAsync));
            ProxyReference.Tournament tournament = await _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            tournamentPerformanceMeter.Stop();
            var pageHeaderDto = _mapper.Map<PageHeaderDto>(tournament);

            if (string.IsNullOrEmpty(pageHeaderDto.LogoUrl))
            {
                using var organizationPerformanceMeter = new PerformanceMeter(
                    _logger,
                    MeasurementCategory.PageHeaderMeasurement,
                    nameof(_organizationService.GetOrganizationLogoUrlAsync));
                pageHeaderDto.LogoUrl = await _organizationService.GetOrganizationLogoUrlAsync(organizationId);
                organizationPerformanceMeter.Stop();
            }

            pageHeaderDto.DisplayWildcards = tournament.AgeGroups
                .Where(x => x.Flights != null)
                .SelectMany(x => x.Flights)
                .Any(x => x.DisplayWildcards);

            return pageHeaderDto;
        }
    }
}
