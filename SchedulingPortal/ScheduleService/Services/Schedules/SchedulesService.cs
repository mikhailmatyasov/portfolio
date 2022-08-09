using AutoMapper;
using Common.Extensions;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.Schedules;
using Model.Dto.Schedules.FilterData;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess;
using ScheduleService.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.Schedules
{
    public class SchedulesService : ISchedulesService
    {
        private readonly ITournamentVenuesProvider _tournamentVenuesProvider;
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IFlightGamesProvider _flightGamesProvider;
        private readonly IFlightStandingsProvider _tournamentFlightStandingsProvider;
        private readonly ILeaguesAndClubsProvider _leaguesAndClubsProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<SchedulesService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulesService"/> class.
        /// </summary>
        /// <param name="tournamentVenuesProvider">Tournament venues data provider.</param>
        /// <param name="tournamentProvider">Tournament data provider.</param>
        /// <param name="flightGamesProvider">FlightGames data provider.</param>
        /// <param name="tournamentFlightStandingsProvider">Tournament flight standings data provider.</param>
        /// <param name="leaguesAndClubsProvider">Leagues and clubs data provider.</param>
        /// <param name="mapper">Mapper to convert into DTOs.</param>
        /// <param name="logger">Logger.</param>
        public SchedulesService(
            ITournamentVenuesProvider tournamentVenuesProvider,
            ITournamentProvider tournamentProvider,
            IFlightGamesProvider flightGamesProvider,
            IFlightStandingsProvider tournamentFlightStandingsProvider,
            ILeaguesAndClubsProvider leaguesAndClubsProvider,
            IMapper mapper,
            ILogger<SchedulesService> logger)
        {
            _tournamentVenuesProvider = tournamentVenuesProvider;
            _tournamentProvider = tournamentProvider;
            _flightGamesProvider = flightGamesProvider;
            _tournamentFlightStandingsProvider = tournamentFlightStandingsProvider;
            _leaguesAndClubsProvider = leaguesAndClubsProvider;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets Schedules data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>TournamentFlightGames ungrouped/flat mapped data representing schedules (Brackets).</returns>
        public async Task<ScheduleDto> GetSchedulesAsync(string organizationId, string tournamentId)
        {
            var tournamentVenuesTask = _tournamentVenuesProvider.GetTournamentVenuesAsync(organizationId, tournamentId);
            var tournamentTask = _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            var leaguesWithClubsTask = _leaguesAndClubsProvider.GetOrganizationLeaguesWithClubsAsync(organizationId);
            var externalMethodNames = nameof(_tournamentVenuesProvider.GetTournamentVenuesAsync) + " " +
                                      nameof(_tournamentProvider.GetTournamentAsync) + " " +
                                      nameof(_leaguesAndClubsProvider.GetOrganizationLeaguesWithClubsAsync) + " " +
                                      nameof(_tournamentFlightStandingsProvider.GetFlightStandingsListAsync) + " " +
                                      nameof(_flightGamesProvider.GetFlightGamesListAsync);
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.SchedulesMeasurement,
                externalMethodNames);
            TournamentVenues tournamentVenues = await tournamentVenuesTask;
            ProxyReference.Tournament tournament = await tournamentTask;
            var flightIds = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(x => x.Flights)
                .Select(flight => new KeyValuePair<string, DateTime>(flight.Key, flight.LastUpdated))
                .Where(pair => !string.IsNullOrEmpty(pair.Key))
                .ToArray();
            Task<IEnumerable<TournamentFlightStandings>> flightStandingsListTask =
                _tournamentFlightStandingsProvider.GetFlightStandingsListAsync(organizationId, tournamentId, flightIds);
            Task<IEnumerable<FlightGames>> flightGamesTask = _flightGamesProvider.GetFlightGamesListAsync(organizationId, tournamentId, flightIds);
            IEnumerable<TournamentFlightStandings> flightStandingsList = await flightStandingsListTask;
            IEnumerable<FlightGames> flightGames = (await flightGamesTask).ToArray();

            // Dates and Ladders
            IEnumerable<TournamentRound> tournamentRounds = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(x => x.Flights)
                .Where(x => x.Rounds != null)
                .SelectMany(tf => tf.Rounds)
                .ToArray();
            var tournamentRoundsGrouped = tournamentRounds.GroupBy(tr => tr.RoundType);
            var tournamentRoundsGroupedDto = tournamentRoundsGrouped.Select(tr => new TournamentRoundsSoapDto()
            {
                LadderStep = tr.Key,
                Rounds = tr.ToArray(),
            });
            List<PlayDto> dates = new();
            List<PlayDto> ladders = new();

            // Dates & Ladders
            foreach (TournamentRoundsSoapDto rounds in tournamentRoundsGroupedDto)
            {
                foreach (TournamentRound round in rounds.Rounds)
                {
                    TournamentGame[] roundGames = flightGames
                        .SelectMany(fg => fg.Games)
                        .Where(game => round.Groups.Any(group => group.Key == game.RoundGroupKey))
                        .ToArray();

                    // Dates
                    IEnumerable<PlayDto> newDates = roundGames
                        .Select(tg => ScheduleHelper.GameToSchedulesMap(tg, tournamentVenues, round, _mapper));
                    dates.AddRange(newDates);

                    // Ladders
                    if (rounds.LadderStep != "B" && rounds.LadderStep != "C")
                    {
                        ladders.AddRange(roundGames
                                .Select(tg => ScheduleHelper.GameToSchedulesMap(tg, tournamentVenues, round, _mapper)));
                    }
                }
            }

            // Brackets
            var tournamentRoundGroupStandings = flightStandingsList
                .SelectMany(standings => standings.RoundStandings)
                .SelectMany(trs => trs.GroupStandings.Where(trgs => trgs.Standings != null))
                .ToArray();
            var tournamentRoundGroups = tournamentRounds
                .SelectMany(tr => tr.Groups)
                .Where(trg => tournamentRoundGroupStandings.Any(trgs => trgs.RoundKey == trg.RoundKey));
            var brackets = tournamentRoundGroupStandings.SelectMany(trgs => trgs.Standings
                .Where(x => !x.TeamKey.IsEmptyGuid())
                .Select(ts => new ScheduleBracketDto
                {
                    ClubKey = ts.ClubKey,
                    FlightKey = ts.FlightKey,
                    GroupCode = tournamentRoundGroups.FirstOrDefault(trg => trg.Key == trgs.RoundGroupKey)?.GroupCode,
                    TeamKey = ts.TeamKey,
                    SubGroup = ts.SeatDescription,
                    Games = ts.Games.Select(x => new BracketGameDto
                    {
                        Score = x.Score,
                        IsForfeit = x.IsForfeit,
                        IsPlayed = x.IsPlayed,
                    }).ToList(),
                    TeamName = ts.TeamName,
                    TotalPoints = ts.TotalScore,
                    TieBreakValues = ts.TieBreaks?.Where(x => x.Rule.IsDisplayed).Select(x => x.Score).ToList() ?? new List<int>(),
                    YellowCards = ts.YellowCards,
                    RedCards = ts.RedCards,
                }));
            var tieBreaksColumns = tournamentRoundGroupStandings.FirstOrDefault()?
                .Standings.FirstOrDefault()?
                .TieBreaks?
                .Where(x => x.Rule.IsDisplayed)
                .Select(x => x.Rule.Label);

            // Filter data
            IDictionary<string, string> flightFilterData = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(x => x.Flights)
                .ToDictionary(flight => flight.Key, flight => flight.FlightName);
            IDictionary<string, VenueFieldsDto> venueFieldFilterData = tournamentVenues.Venues
                .Select(venue => new VenueFieldsDto
                {
                    VenueKey = venue.RecordKey,
                    VenueName = venue.Name,
                    Fields = venue.Fields.ToDictionary(field => field.RecordKey, field => field.Name),
                })
                .ToDictionary(dto => dto.VenueKey);
            var flightGroupPairs = dates
                .SelectMany(dto => new[]
                {
                    new KeyValuePair<string, TeamDto>(dto.FlightKey, dto.HomeTeam),
                    new KeyValuePair<string, TeamDto>(dto.FlightKey, dto.AwayTeam),
                })
                .Where(kvp => !string.IsNullOrEmpty(kvp.Value.TeamGroupCode) && !kvp.Value.Id.IsEmptyGuid())
                .Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value.TeamGroupCode))
                .Distinct()
                .OrderBy(kvp => kvp.Value)
                .ToArray();
            var flightGroups = new Dictionary<string, HashSet<string>> { { string.Empty, new HashSet<string>() } };
            foreach (KeyValuePair<string, string> flightGroupPair in flightGroupPairs)
            {
                flightGroups[string.Empty].AddIfNotExist(flightGroupPair.Value);
                flightGroups.GetOrCreate(flightGroupPair.Key).AddIfNotExist(flightGroupPair.Value);
            }

            Dictionary<string, string> scheduleClubIds = dates
                .SelectMany(dto => new[] { dto.HomeTeam.ClubId, dto.AwayTeam.ClubId })
                .Distinct()
                .Where(clubId => !string.IsNullOrEmpty(clubId))
                .ToDictionary(clubId => clubId, clubId => clubId);
            OrganizationLeaguesWithClubs leaguesWithClubs = await leaguesWithClubsTask;
            performanceMeter.Stop();
            IDictionary<string, string> scheduleClubs = leaguesWithClubs.LeagueWithClubs
                .Where(lwc => lwc.Clubs != null)
                .SelectMany(lwc => lwc.Clubs)
                .Where(club => scheduleClubIds.ContainsKey(club.RecordKey))
                .ToDictionary(club => club.RecordKey, club => club.Name);
            SchedulesFilterDto filterData = new()
            {
                Clubs = scheduleClubs,
                FlightGroups = flightGroups
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AsEnumerable()),
                Flights = flightFilterData,
                VenueFields = venueFieldFilterData,
            };

            // Result
            var result = new ScheduleDto
            {
                DatesGroupedDto = dates.GroupBy(pDto => pDto.PlayDateTime.Date).Select(x => new DateGroupedDto
                {
                    PlayDateTime = x.Key,
                    SchedulesDto = x.ToList(),
                }).OrderBy(x => x.PlayDateTime),
                LaddersFlightGroupedDto = ladders.GroupBy(pDto => pDto.FlightKey)
                .Select(flightGroupedDto => new LadderFlightGroupedDto
                {
                    FlightKey = flightGroupedDto.Key,
                    LaddersRoundGroupedDto = flightGroupedDto.GroupBy(pDto => pDto.RoundName)
                    .Select(roundGroupedDto => new LadderRoundGroupedDto
                    {
                        LadderStep = roundGroupedDto.Key,
                        LaddersDto = roundGroupedDto.ToList(),
                    }),
                }),
                BracketsFlightGroupedDto = brackets.GroupBy(sbDto => sbDto.FlightKey)
                .Select(flightGroupedDto => new BracketFlightGroupedDto
                {
                    FlightKey = flightGroupedDto.Key,
                    BracketsGroupCodeGroupedDto = flightGroupedDto.GroupBy(pDto => pDto.GroupCode)
                    .Select(gcGroupedDto => new BracketGroupCodeGroupedDto
                    {
                        GroupCode = gcGroupedDto.Key,
                        BracketsDto = gcGroupedDto.ToList(),
                    }),
                }),
                TieBreaksColumns = tieBreaksColumns ?? Array.Empty<string>(),

                FilterData = filterData,
            };
            return result;
        }

        private class TournamentRoundsSoapDto
        {
            public string LadderStep { get; init; }

            public IEnumerable<TournamentRound> Rounds { get; init; }
        }
    }
}
