using AutoMapper;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.ClubInfo;
using Model.Dto.Standings;
using ScheduleService.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.ClubInfo
{
    public class ClubInfoService : IClubInfoService
    {
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IFlightStandingsProvider _flightStandingsProvider;
        private readonly ILogger<ClubInfoService> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClubInfoService"/> class.
        /// </summary>
        /// <param name="tournamentProvider">Tournament data provider.</param>
        /// <param name="flightStandingsProvider">Flight standings provider.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="mapper">Mapper to convert into DTOs.</param>
        public ClubInfoService(
            ITournamentProvider tournamentProvider,
            IFlightStandingsProvider flightStandingsProvider,
            ILogger<ClubInfoService> logger,
            IMapper mapper)
        {
            _tournamentProvider = tournamentProvider;
            _flightStandingsProvider = flightStandingsProvider;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ClubInfoDto> GetClubInfoAsync(string organizationId, string tournamentId, string clubId)
        {
            using var tournamentPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.ClubInfoMeasurement,
                nameof(_tournamentProvider.GetTournamentAsync));
            ProxyReference.Tournament tournament = await _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            tournamentPerformanceMeter.Stop();

            var flightIds = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(x => x.Flights)
                .Select(flight => new KeyValuePair<string, DateTime>(flight.Key, flight.LastUpdated))
                .Where(pair => !string.IsNullOrEmpty(pair.Key))
                .ToArray();

            using var flightStandingsListPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.ClubInfoMeasurement,
                nameof(_flightStandingsProvider.GetFlightStandingsListAsync));
            var flightStandings = await _flightStandingsProvider.GetFlightStandingsListAsync(organizationId, tournamentId, flightIds);
            flightStandingsListPerformanceMeter.Stop();

            var teamStandings = flightStandings
                .Where(x => x.RoundStandings != null)
                .Select(x => x.RoundStandings.First())
                .SelectMany(x => x.GroupStandings)
                .Where(x => x.Standings != null)
                .SelectMany(x => x.Standings)
                .Where(x => string.Equals(x.ClubKey, clubId, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.TotalScore)
                .ToArray();

            if (!teamStandings.Any())
            {
                return new ClubInfoDto
                {
                    Name = "Unknown club",
                    TeamStandings = Array.Empty<TeamStandingsDto>(),
                };
            }

            return new ClubInfoDto
            {
                Name = teamStandings.First().ClubName,
                TeamStandings = _mapper.Map<IEnumerable<TeamStandingsDto>>(teamStandings),
            };
        }
    }
}
