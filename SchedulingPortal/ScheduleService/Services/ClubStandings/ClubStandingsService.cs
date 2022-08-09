using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.ClubStandings;
using ProxyReference;
using ScheduleService.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.ClubStandings
{
    public class ClubStandingsService : IClubStandingsService
    {
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IFlightStandingsProvider _flightStandingsProvider;
        private readonly ILogger<ClubStandingsService> _logger;

        public ClubStandingsService(
            ITournamentProvider tournamentProvider,
            IFlightStandingsProvider flightStandingsProvider,
            ILogger<ClubStandingsService> logger)
        {
            _tournamentProvider = tournamentProvider;
            _flightStandingsProvider = flightStandingsProvider;
            _logger = logger;
        }

        /// <summary>
        /// Gets Club Standings data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>ClubStandings mapped data.</returns>
        public async Task<IEnumerable<ClubStandingsDto>> GetClubStandingsAsync(string organizationId, string tournamentId)
        {
            using var tournamentPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.ClubStandingsMeasurement,
                nameof(_tournamentProvider.GetTournamentAsync));
            ProxyReference.Tournament tournament = await _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            tournamentPerformanceMeter.Stop();

            var flightIds = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(x => x.Flights)
                .Select(flight => new KeyValuePair<string, DateTime>(flight.Key, flight.LastUpdated))
                .Where(pair => !string.IsNullOrEmpty(pair.Key))
                .ToArray();

            using var flightStandingsPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.ClubStandingsMeasurement,
                nameof(_flightStandingsProvider.GetFlightStandingsListAsync));
            var flightStandings = await _flightStandingsProvider.GetFlightStandingsListAsync(organizationId, tournamentId, flightIds);
            flightStandingsPerformanceMeter.Stop();

            var teamStandings = flightStandings
                .Where(x => x.RoundStandings != null)
                .Select(x => x.RoundStandings.First())
                .SelectMany(x => x.GroupStandings)
                .Where(x => x.Standings != null)
                .SelectMany(x => x.Standings)
                .Where(x => !string.IsNullOrEmpty(x.ClubKey));

            var result = GroupTeamStandingsToClubStandings(teamStandings)
                .OrderByDescending(x => x.GamesPlayed);

            return result;
        }

        private static IEnumerable<ClubStandingsDto> GroupTeamStandingsToClubStandings(
            IEnumerable<TournamentStanding> teamStandings)
        {
            return teamStandings
                .GroupBy(x => x.ClubKey)
                .Select(g => new ClubStandingsDto
                {
                    Id = g.Key,
                    Name = g.First().ClubName,
                    GamesPlayed = g.Sum(x => x.TotalGames),
                    Wins = g.Sum(x => x.Wins),
                    Losses = g.Sum(x => x.Losses),
                    Ties = g.Sum(x => x.Ties),
                    GoalsFor = g.Sum(x => x.GoalsScoredFor),
                    GoalsAgainst = g.Sum(x => x.GoalsScoredAgainst),
                    YellowCards = g.Sum(x => x.YellowCards),
                    RedCards = g.Sum(x => x.RedCards),
                }).ToList();
        }
    }
}
