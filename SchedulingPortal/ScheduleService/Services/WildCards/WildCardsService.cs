using Common.Extensions;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.WildCards;
using ScheduleService.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.WildCards
{
    public class WildCardsService : IWildCardsService
    {
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IFlightStandingsProvider _tournamentFlightStandingsProvider;
        private readonly ILogger<WildCardsService> _logger;

        public WildCardsService(
            ITournamentProvider tournamentProvider,
            IFlightStandingsProvider tournamentFlightStandingsProvider,
            ILogger<WildCardsService> logger)
        {
            _tournamentProvider = tournamentProvider;
            _tournamentFlightStandingsProvider = tournamentFlightStandingsProvider;
            _logger = logger;
        }

        public async Task<WildCardsDto> GetWildCardsAsync(string organizationId, string tournamentId)
        {
            using var tournamentPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.WildCardsMeasurement,
                nameof(_tournamentProvider.GetTournamentAsync));
            ProxyReference.Tournament tournament = await _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            tournamentPerformanceMeter.Stop();

            var flightIds = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(x => x.Flights)
                .Where(x => x.DisplayWildcards)
                .Select(flight => new KeyValuePair<string, DateTime>(flight.Key, flight.LastUpdated))
                .Where(pair => !string.IsNullOrEmpty(pair.Key))
                .ToArray();

            var flightStandingsTask = _tournamentFlightStandingsProvider.GetFlightStandingsListAsync(organizationId, tournamentId, flightIds);

            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.WildCardsMeasurement,
                nameof(_tournamentFlightStandingsProvider.GetFlightStandingsListAsync));
            var flightStandings = (await flightStandingsTask).ToArray();
            performanceMeter.Stop();

            var tournamentRoundGroupStandings = flightStandings
                .SelectMany(standings => standings.RoundStandings)
                .SelectMany(trs => trs.GroupStandings.Where(x => x.Standings != null))
                .ToArray();

            var wildCardItems = tournamentRoundGroupStandings.SelectMany(x => x.Standings
                .Where(ts => ts.WildcardStanding > 0)
                .Where(ts => !ts.TeamKey.IsEmptyGuid())
                .Select(ts => new WildCardItemDto
                {
                    Number = ts.WildcardStanding,
                    Tie = ts.WildcardTie,
                    Advanced = ts.WildcardAdvances,
                    SubGroup = ts.SeatDescription,
                    FlightKey = ts.FlightKey,
                    ClubKey = ts.ClubKey,
                    TeamKey = ts.TeamKey,
                    TeamName = ts.TeamName,
                    TotalPoints = ts.TotalScore,
                    TieBreakValues = ts.TieBreaks?.Where(tb => tb.Rule.IsDisplayed).Select(tb => tb.Score).ToList() ??
                                     new List<int>(),
                }));

            var wildCardsAgeGroups = flightStandings
                .Where(x => wildCardItems.Any(wc => wc.FlightKey == x.FlightKey))
                .Select(x => new WildCardsAgeGroupDto
                {
                    AgeGroupKey = x.AgeGroupKey,
                    AgeGroupName = tournament.AgeGroups
                        .Where(ag => ag.Flights != null)
                        .SelectMany(ag => ag.Flights)
                        .First(tf => tf.Key == x.FlightKey).FlightName,
                    WildCards = wildCardItems.Where(wc => wc.FlightKey == x.FlightKey)
                        .OrderBy(wc => wc.Number),
                });

            var tieBreaksColumns = tournamentRoundGroupStandings.FirstOrDefault()?
                .Standings.FirstOrDefault()?
                .TieBreaks?
                .Where(x => x.Rule.IsDisplayed)
                .Select(x => x.Rule.Label);

            return new WildCardsDto
            {
                WildCardsAgeGroups = wildCardsAgeGroups,
                TieBreaksColumns = tieBreaksColumns,
            };
        }
    }
}
