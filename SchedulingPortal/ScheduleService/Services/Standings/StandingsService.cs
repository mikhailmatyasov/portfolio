using AutoMapper;
using Common.Extensions;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.Schedules;
using ProxyReference;
using ScheduleService.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.Standings
{
    public class StandingsService : IStandingsService
    {
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IFlightStandingsProvider _flightStandingsProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<StandingsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandingsService"/> class.
        /// </summary>
        /// <param name="tournamentProvider">Tournament data provider.</param>
        /// <param name="flightStandingsProvider">Flight standings provider.</param>
        /// <param name="mapper">Mapper to convert into DTOs.</param>
        /// <param name="logger">Logger.</param>
        public StandingsService(
            ITournamentProvider tournamentProvider,
            IFlightStandingsProvider flightStandingsProvider,
            IMapper mapper,
            ILogger<StandingsService> logger)
        {
            _tournamentProvider = tournamentProvider;
            _flightStandingsProvider = flightStandingsProvider;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<StandingsBracketGroupedDto>> GetStandingsAsync(string organizationId, string tournamentId)
        {
            using var tournamentPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.StandingsMeasurement,
                nameof(_tournamentProvider.GetTournamentAsync));
            ProxyReference.Tournament tournament =
                await _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            tournamentPerformanceMeter.Stop();

            var flightIds = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(x => x.Flights)
                .Select(flight => new KeyValuePair<string, DateTime>(flight.Key, flight.LastUpdated))
                .Where(pair => !string.IsNullOrEmpty(pair.Key))
                .ToArray();

            using var flightStandingsPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.StandingsMeasurement,
                nameof(_flightStandingsProvider.GetFlightStandingsListAsync));
            var allFlightStandings =
                (await _flightStandingsProvider.GetFlightStandingsListAsync(organizationId, tournamentId, flightIds))
                .Where(x => x.RoundStandings != null && x.RoundStandings.Any())
                .ToList();
            flightStandingsPerformanceMeter.Stop();

            var result = new List<StandingsBracketGroupedDto>();
            foreach (TournamentFlightStandings flightStandings in allFlightStandings)
            {
                TournamentFlight tournamentFlight = tournament.AgeGroups
                    .Where(ag => ag.Flights != null)
                    .SelectMany(x => x.Flights)
                    .First(x => x.Key == flightStandings.FlightKey);

                var groupIdToCode = tournamentFlight.Rounds.First().Groups
                    .GroupBy(g => g.Key)
                    .Select(groups => groups.First())
                    .ToDictionary(x => x.Key, v => v.GroupCode);

                AgeGroup ageGroup = tournament.AgeGroups
                    .First(x => x.AgeGroup.Key == tournamentFlight.AgeGroupKey)
                    .AgeGroup;

                var roundStandings = flightStandings.RoundStandings;
                var groupStandings = roundStandings.First().GroupStandings
                    .Where(trgs => trgs.Standings != null);
                result.AddRange(groupStandings.Select(x => new StandingsBracketGroupedDto
                {
                    AgeGroupKey = ageGroup.Key,
                    AgeGroupName = ageGroup.AgeName,
                    GroupCode = groupIdToCode[x.RoundGroupKey],
                    BracketsDto = _mapper
                        .Map<IEnumerable<StandingsBracketDto>>(x.Standings.Where(s => !s.TeamKey.IsEmptyGuid()))
                        .OrderByDescending(ts => ts.GamesPlayed),
                }));
            }

            return result;
        }
    }
}
