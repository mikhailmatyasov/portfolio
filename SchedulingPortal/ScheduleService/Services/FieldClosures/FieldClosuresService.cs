using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.FieldClosures;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.FieldClosures
{
    public class FieldClosuresService : IFieldClosuresService
    {
        private readonly ITournamentVenuesProvider _tournamentVenuesProvider;
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IFlightGamesProvider _flightGamesProvider;
        private readonly ILogger<FieldClosuresService> _logger;

        public FieldClosuresService(
            ITournamentVenuesProvider tournamentVenuesProvider,
            ITournamentProvider tournamentProvider,
            IFlightGamesProvider flightGamesProvider,
            ILogger<FieldClosuresService> logger)
        {
            _tournamentVenuesProvider = tournamentVenuesProvider;
            _tournamentProvider = tournamentProvider;
            _flightGamesProvider = flightGamesProvider;
            _logger = logger;
        }

        /// <summary>
        /// Gets field closures data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>FieldClosure mapped data.</returns>
        public async Task<IEnumerable<FieldClosuresDto>> GetFieldClosuresAsync(
            string organizationId,
            string tournamentId)
        {
            var tournamentVenuesTask = _tournamentVenuesProvider.GetTournamentVenuesAsync(organizationId, tournamentId);
            var tournamentTask = _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);

            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.FieldClosuresMeasurement,
                $"{nameof(_tournamentVenuesProvider.GetTournamentVenuesAsync)} {nameof(_tournamentProvider.GetTournamentAsync)}");
            TournamentVenues tournamentVenues = await tournamentVenuesTask;
            ProxyReference.Tournament tournament = await tournamentTask;
            performanceMeter.Stop();
            var tournamentFlights = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(ag => ag.Flights)
                .ToArray();

            var flightIds = tournamentFlights
                .Select(flight => new KeyValuePair<string, DateTime>(flight.Key, flight.LastUpdated))
                .Where(pair => !string.IsNullOrEmpty(pair.Key))
                .ToArray();
            using var flightGamesPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.FieldClosuresMeasurement,
                nameof(_flightGamesProvider.GetFlightGamesListAsync));
            var flightGames = (await _flightGamesProvider.GetFlightGamesListAsync(organizationId, tournamentId, flightIds)).ToArray();
            flightGamesPerformanceMeter.Stop();

            var fieldsWithClosures = tournamentVenues.Venues
                .Where(x => x.Fields != null)
                .SelectMany(x => x.Fields)
                .Where(x => x.Closures != null);

            var tournamentGames = flightGames.SelectMany(x => x.Games).ToList();

            var groupingFieldClosures = new List<GroupingFieldClosure>();
            foreach (TournamentField field in fieldsWithClosures)
            {
                var gamesInField = tournamentGames.Where(x => x.FieldKey == field.RecordKey);

                var affectedClosures = field.Closures
                    .Where(closure => gamesInField.Any(game => IsClosureOverlapsGame(game, closure)))
                    .Select(closure => new FieldClosureFieldDto
                    {
                        Title = field.Name,
                        ClosedFrom = closure.From,
                        ClosedTo = closure.To,
                        AffectedGames = gamesInField
                            .Where(game => IsClosureOverlapsGame(game, closure))
                            .Select(game => new FieldClosureAffectedGamesDto
                            {
                                Group = GetAgeGroupName(game, flightGames, tournamentFlights, tournament),
                                Time = game.StartTime,
                            }),
                    });

                foreach (FieldClosureFieldDto affectedClosure in affectedClosures)
                {
                    groupingFieldClosures.AddRange(
                        affectedClosure.AffectedGames.GroupBy(x => x.Time)
                        .Select(x => new GroupingFieldClosure
                        {
                            VenueId = field.VenueKey,
                            Date = x.Key.Date,
                            Field = affectedClosure,
                        }));
                }
            }

            return groupingFieldClosures.GroupBy(k => (k.VenueId, k.Date), v => v.Field)
                .Select(x => new FieldClosuresDto
                {
                    Place = tournamentVenues.Venues.First(v => v.RecordKey == x.Key.VenueId).ShortName,
                    Date = x.Key.Date,
                    Fields = x.ToList(),
                })
                .OrderByDescending(x => x.Date);
        }

        private static bool IsClosureOverlapsGame(TournamentGame game, FieldClosure closure)
        {
            return closure.From <= game.StartTime && closure.To > game.StartTime;
        }

        private static string GetAgeGroupName(
            TournamentGame game,
            IEnumerable<FlightGames> flightGames,
            IEnumerable<TournamentFlight> tournamentFlights,
            ProxyReference.Tournament tournament)
        {
            string gameFlightKey = flightGames.Single(x => x.Games.Contains(game)).FlightKey;
            TournamentFlight tournamentFlight = tournamentFlights.Single(fl => fl.Key == gameFlightKey);
            AgeGroup ageGroup = tournament.AgeGroups.Single(x => x.Flights.Contains(tournamentFlight)).AgeGroup;

            return ageGroup.AgeName;
        }

        private class GroupingFieldClosure
        {
            public string VenueId { get; init; }

            public DateTime Date { get; init; }

            public FieldClosureFieldDto Field { get; init; }
        }
    }
}
