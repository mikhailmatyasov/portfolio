using AutoMapper;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.Tournament;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess;
using ScheduleService.Services.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.Tournament
{
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IOrganizationGendersProvider _organizationGendersProvider;
        private readonly IOrganizationService _organizationService;
        private readonly ILogger<TournamentService> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentService"/> class.
        /// </summary>
        /// <param name="tournamentProvider">Tournament data provider.</param>
        /// <param name="organizationGendersProvider">Organizations genders provider.</param>
        /// <param name="mapper">Mapper to convert into DTOs.</param>
        /// <param name="organizationService">Organization service.</param>
        /// <param name="logger">Logger.</param>
        public TournamentService(
            ITournamentProvider tournamentProvider,
            IOrganizationGendersProvider organizationGendersProvider,
            IMapper mapper,
            IOrganizationService organizationService,
            ILogger<TournamentService> logger)
        {
            _tournamentProvider = tournamentProvider;
            _organizationGendersProvider = organizationGendersProvider;
            _mapper = mapper;
            _organizationService = organizationService;
            _logger = logger;
        }

        /// <summary>
        /// Gets tournament data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Tournament info.</returns>
        public async Task<TournamentDto> GetTournamentAsync(string organizationId, string tournamentId)
        {
            Task<OrganizationGenders> getGendersTask = _organizationGendersProvider.GetOrganizationGendersAsync(organizationId);
            Task<ProxyReference.Tournament> getTournamentTask = _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            var externalMethodNames = nameof(_tournamentProvider.GetTournamentAsync) + " " + nameof(_organizationGendersProvider.GetOrganizationGendersAsync);
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.TournamentMeasurement,
                externalMethodNames);
            ProxyReference.Tournament tournament = await getTournamentTask;
            OrganizationGenders genders = await getGendersTask;
            performanceMeter.Stop();
            var tournamentDto = _mapper.Map<TournamentDto>(tournament);
            var genderAgeGroups = new List<GenderFlightsDto>();
            foreach (var gender in genders.Genders.OrderBy(gender => gender.PluralLabel))
            {
                IEnumerable<TournamentFlight> flights = tournament.AgeGroups
                    .Where(ag => ag.AgeGroup.GenderCode == gender.Code && ag.Flights != null)
                    .SelectMany(ag => ag.Flights);
                if (flights.Any())
                {
                    genderAgeGroups.Add(new GenderFlightsDto()
                    {
                        GenderPluralLabel = gender.PluralLabel,
                        Flights = _mapper.Map<TournamentFlightDto[]>(flights),
                    });
                }
            }

            tournamentDto.Genders = genderAgeGroups;
            if (string.IsNullOrEmpty(tournamentDto.LogoUrl))
            {
                using var organizationLogoPerformanceMeter = new PerformanceMeter(
                    _logger,
                    MeasurementCategory.TournamentMeasurement,
                    nameof(_organizationService.GetOrganizationLogoUrlAsync));
                tournamentDto.LogoUrl = await _organizationService.GetOrganizationLogoUrlAsync(organizationId);
                organizationLogoPerformanceMeter.Stop();
            }

            tournamentDto.DisplayWildcards = tournament.AgeGroups
                .Where(x => x.Flights != null)
                .SelectMany(x => x.Flights)
                .Any(x => x.DisplayWildcards);

            return tournamentDto;
        }
    }
}
