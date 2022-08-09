using AutoMapper;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.Venues;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.Venues
{
    public class VenuesService : IVenuesService
    {
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IFlightGamesProvider _flightGamesProvider;
        private readonly ITournamentVenuesProvider _tournamentVenuesProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<VenuesService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenuesService"/> class.
        /// </summary>
        /// <param name="tournamentProvider">Tournament data provider.</param>
        /// <param name="flightGamesProvider">FlightGames data provider.</param>
        /// <param name="tournamentVenuesProvider">TournamentVenues data provider.</param>
        /// <param name="mapper">Mapper to convert into DTOs.</param>
        /// <param name="logger">Logger.</param>
        public VenuesService(
            ITournamentProvider tournamentProvider,
            IFlightGamesProvider flightGamesProvider,
            ITournamentVenuesProvider tournamentVenuesProvider,
            IMapper mapper,
            ILogger<VenuesService> logger)
        {
            _tournamentProvider = tournamentProvider;
            _flightGamesProvider = flightGamesProvider;
            _tournamentVenuesProvider = tournamentVenuesProvider;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets venues data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Venues.</returns>
        public async Task<VenuesDto> GetVenuesAsync(
            string organizationId,
            string tournamentId)
        {
            Task<ProxyReference.Tournament> tournamentTask = _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            Task<TournamentVenues> tournamentVenuesTask =
                _tournamentVenuesProvider.GetTournamentVenuesAsync(organizationId, tournamentId);
            var externalMethodNames = nameof(_tournamentProvider.GetTournamentAsync) + " " +
                                      nameof(_tournamentVenuesProvider.GetTournamentVenuesAsync) + " " +
                                      nameof(_flightGamesProvider.GetFlightGamesListAsync);
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.VenuesMeasurement,
                externalMethodNames);
            ProxyReference.Tournament tournament = await tournamentTask;
            var flightIds = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(group => group.Flights)
                .Select(flight => new KeyValuePair<string, DateTime>(flight.Key, flight.LastUpdated))
                .Where(pair => !string.IsNullOrEmpty(pair.Key))
                .ToArray();
            IEnumerable<FlightGames> flightGames = (await _flightGamesProvider.GetFlightGamesListAsync(organizationId, tournamentId, flightIds)).ToArray();
            TournamentVenues tournamentVenues = await tournamentVenuesTask;
            performanceMeter.Stop();
            TournamentFlight[] flights = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(ag => ag.Flights)
                .ToArray();
            IEnumerable<VenuesFlightDto> flightDtos = _mapper.Map<IEnumerable<VenuesFlightDto>>(flights).ToArray();

            foreach (VenuesRoundDto venuesRoundDto in flightDtos.SelectMany(flight => flight.Rounds))
            {
                var round = flights
                    .Where(flight => flight.Rounds != null)
                    .SelectMany(flight => flight.Rounds)
                    .First(r => r.Key == venuesRoundDto.Key);
                TournamentGame[] games = flightGames
                    .SelectMany(fg => fg.Games)
                    .Where(game => round.Groups.Any(group => group.Key == game.RoundGroupKey))
                    .ToArray();
                IOrderedEnumerable<DateTime> dates = games
                    .Select(game => game.StartTime.Date)
                    .Distinct()
                    .OrderBy(date => date);
                IOrderedEnumerable<TournamentVenue> venues = tournamentVenues.Venues
                    .Where(venue => venue.Fields.Any(field => games.Any(game => game.FieldKey == field.RecordKey)))
                    .OrderBy(venue => venue.ShortName);
                venuesRoundDto.Dates = dates;
                venuesRoundDto.Venues = _mapper.Map<IEnumerable<VenuesVenueDto>>(venues);
            }

            return new VenuesDto { Flights = FilterEmptyFlights(flightDtos) };
        }

        private IEnumerable<VenuesFlightDto> FilterEmptyFlights(IEnumerable<VenuesFlightDto> flights)
        {
            IEnumerable<VenuesFlightDto> venuesFlightDtos = flights as VenuesFlightDto[] ?? flights.ToArray();
            foreach (var flight in venuesFlightDtos)
            {
                flight.Rounds = flight.Rounds.Where(fr => fr.Dates.Any());
            }

            return venuesFlightDtos.Where(flight => flight.Rounds.Any());
        }
    }
}
