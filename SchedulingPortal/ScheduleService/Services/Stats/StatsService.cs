using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.Stats;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.Stats
{
    public class StatsService : IStatsService
    {
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IFlightGamesProvider _flightGamesProvider;
        private readonly ITournamentTopScorerProvider _tournamentTopScorerProvider;
        private readonly ILogger<StatsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatsService"/> class.
        /// </summary>
        /// <param name="tournamentProvider">Tournament data provider.</param>
        /// <param name="flightGamesProvider">FlightGames data provider.</param>
        /// <param name="tournamentTopScorerProvider">TournamentTopScorer data provider.</param>
        /// <param name="logger">Logger.</param>
        public StatsService(
            ITournamentProvider tournamentProvider,
            IFlightGamesProvider flightGamesProvider,
            ITournamentTopScorerProvider tournamentTopScorerProvider,
            ILogger<StatsService> logger)
        {
            _tournamentProvider = tournamentProvider;
            _flightGamesProvider = flightGamesProvider;
            _tournamentTopScorerProvider = tournamentTopScorerProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<StatDto>> GetStatsAsync(string organizationId, string tournamentId)
        {
            using var tournamentPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.StatsMeasurement,
                nameof(_tournamentProvider.GetTournamentAsync));
            ProxyReference.Tournament tournament = await _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            tournamentPerformanceMeter.Stop();
            var flightIds = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(group => group.Flights)
                .Select(flight => new KeyValuePair<string, DateTime>(flight.Key, flight.LastUpdated))
                .Where(pair => !string.IsNullOrEmpty(pair.Key))
                .ToArray();

            using var flightGamesPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.StatsMeasurement,
                nameof(_flightGamesProvider.GetFlightGamesListAsync));
            IEnumerable<FlightGames> flightGames = (await _flightGamesProvider
                .GetFlightGamesListAsync(organizationId, tournamentId, flightIds))
                .ToArray();
            flightGamesPerformanceMeter.Stop();

            IEnumerable<TournamentTeam> teams = flightGames.SelectMany(fg => fg.Games).Select(tg => tg.HomeTeam)
                .Union(flightGames.SelectMany(fg => fg.Games).Select(tg => tg.AwayTeam))
                .ToArray();
            IEnumerable<string> flights = tournament.AgeGroups
                .Where(tag => tag.Flights != null)
                .SelectMany(tag => tag.Flights)
                .Select(tf => tf.Key)
                .ToArray();
            using var playersWithScoresPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.StatsMeasurement,
                nameof(_tournamentTopScorerProvider.GetPlayersWithScoresAsync));
            IEnumerable<TournamentTopScorer> players = await _tournamentTopScorerProvider.GetPlayersWithScoresAsync(organizationId, tournamentId, flights);
            playersWithScoresPerformanceMeter.Stop();

            List<StatDto> statsDto = new();
            foreach (TournamentTopScorer player in players)
            {
                TournamentTeam team = teams.FirstOrDefault(tt => tt.TeamNumber == player.TeamCode);
                var statDto = new StatDto
                {
                    AgeGroupKey = player.FlightKey,
                    AgeGroupName = tournament.AgeGroups
                        .Where(ag => ag.Flights != null)
                        .SelectMany(tag => tag.Flights)
                        .First(tf => tf.Key == player.FlightKey).FlightName,
                    PlayerNameSurname = player.PlayerName,
                    Team = new TeamDto
                    {
                        TeamName = player.TeamName,
                        TeamClubKey = team != null ? team.ClubKey : string.Empty,
                        TeamKey = team != null ? team.TeamKey : string.Empty,
                    },
                    Goals = player.GoalsScored,
                };
                statsDto.Add(statDto);
            }

            return statsDto;
        }
    }
}
