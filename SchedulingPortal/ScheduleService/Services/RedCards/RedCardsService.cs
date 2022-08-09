using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.RedCards;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.RedCards
{
    public class RedCardsService : IRedCardsService
    {
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IInfractionsProvider _infractionsProvider;
        private readonly IOrganizationGendersProvider _organizationGendersProvider;
        private readonly ILogger<RedCardsService> _logger;

        public RedCardsService(
            ITournamentProvider tournamentProvider,
            IInfractionsProvider infractionsProvider,
            IOrganizationGendersProvider organizationGendersProvider,
            ILogger<RedCardsService> logger)
        {
            _tournamentProvider = tournamentProvider;
            _infractionsProvider = infractionsProvider;
            _organizationGendersProvider = organizationGendersProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<GenderRedCardsDto>> GetRedCardsAsync(string organizationId, string tournamentId)
        {
            var getTournamentTask = _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            var getGendersTask = _organizationGendersProvider.GetOrganizationGendersAsync(organizationId);

            var externalMethodNames = nameof(_tournamentProvider.GetTournamentAsync) + " " +
                                      nameof(_organizationGendersProvider.GetOrganizationGendersAsync) + " " +
                                      nameof(_infractionsProvider.GetTournamentInfractionsAsync);
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.RedCardsMeasurement,
                externalMethodNames);
            ProxyReference.Tournament tournament = await getTournamentTask;
            var tournamentFlights = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(ag => ag.Flights)
                .ToArray();

            string[] flightIds = tournamentFlights
                .Select(flight => flight.Key)
                .Where(key => !string.IsNullOrEmpty(key))
                .ToArray();

            var infractions = (await _infractionsProvider.GetTournamentInfractionsAsync(organizationId, tournamentId, flightIds))
                .ToArray();

            OrganizationGenders genders = await getGendersTask;
            performanceMeter.Stop();

            var redCards = new List<GenderRedCardsDto>();
            foreach (Gender gender in genders.Genders.OrderBy(gender => gender.PluralLabel))
            {
                string[] genderFlightIds = tournament.AgeGroups
                    .Where(ag => ag.AgeGroup.GenderCode == gender.Code && ag.Flights != null)
                    .SelectMany(ag => ag.Flights)
                    .Select(x => x.Key)
                    .ToArray();

                if (genderFlightIds.Any())
                {
                    redCards.Add(new GenderRedCardsDto
                    {
                        GenderPluralLabel = gender.PluralLabel,
                        FlightRedCards = infractions
                            .Where(x => genderFlightIds.Contains(x.FlightKey))
                            .GroupBy(x => x.FlightKey)
                            .Select(g => new FlightRedCardsDto
                            {
                                FlightName = tournamentFlights.First(x => x.Key == g.Key).FlightName,
                                RedCards = g.Select(x => new RedCardDto
                                {
                                    TeamName = x.TeamName,
                                    Date = x.ToDate,
                                    Name = GetName(x),
                                    EjectionReason = x.Infraction?.Name,
                                    NumberOfCards = x.PenaltyAssessed,
                                    NumberOfServedCards = x.PenaltyServed,
                                }),
                            }),
                    });
                }
            }

            return redCards;
        }

        private static string GetName(TournamentInfraction infraction)
        {
            return infraction.IsAdmin
                ? $"{infraction.LastName}, {infraction.FirstName} ({infraction.AdminRole.Name})"
                : $"{infraction.LastName}, {infraction.FirstName} (Player)";
        }
    }
}
