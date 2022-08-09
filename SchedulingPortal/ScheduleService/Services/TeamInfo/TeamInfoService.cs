using AutoMapper;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.Schedules;
using Model.Dto.TeamInfo;
using ProxyReference;
using ScheduleService.CacheEntities;
using ScheduleService.DataAccess;
using ScheduleService.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.TeamInfo
{
    public class TeamInfoService : ITeamInfoService
    {
        private readonly ITournamentProvider _tournamentProvider;
        private readonly IFlightGamesProvider _flightGamesProvider;
        private readonly ITournamentVenuesProvider _tournamentVenuesProvider;
        private readonly ITournamentTeamProvider _tournamentTeamProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<TeamInfoService> _logger;

        public TeamInfoService(
            ITournamentProvider tournamentProvider,
            IFlightGamesProvider flightGamesProvider,
            ITournamentVenuesProvider tournamentVenuesProvider,
            ITournamentTeamProvider tournamentTeamProvider,
            IMapper mapper,
            ILogger<TeamInfoService> logger)
        {
            _tournamentProvider = tournamentProvider;
            _flightGamesProvider = flightGamesProvider;
            _tournamentVenuesProvider = tournamentVenuesProvider;
            _tournamentTeamProvider = tournamentTeamProvider;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TeamInfoDto> GetTeamInfoAsync(string organizationId, string tournamentId, string teamId)
        {
            var teamTask = _tournamentTeamProvider.GetTournamentTeamAsync(organizationId, tournamentId, teamId);
            var scheduleTask = GetScheduleAsync(organizationId, tournamentId, teamId);

            var externalMethodNames = nameof(_tournamentTeamProvider.GetTournamentTeamAsync) + " " +
                                      nameof(_tournamentProvider.GetTournamentAsync) + " " +
                                      nameof(_tournamentVenuesProvider.GetTournamentVenuesAsync) + " " +
                                      nameof(_flightGamesProvider.GetFlightGamesListAsync);
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.TeamInfoMeasurement,
                externalMethodNames);
            TeamWithAdminsPlayers3 team = await teamTask;
            var teamStaff = GetTeamStaff(team);

            IEnumerable<DateGroupedDto> schedule = await scheduleTask;
            performanceMeter.Stop();

            return new TeamInfoDto
            {
                Name = team.TeamName,
                Schedule = schedule,
                Staff = teamStaff,
            };
        }

        private static IEnumerable<TeamInfoStaffDto> GetTeamStaff(TeamWithAdminsPlayers3 team)
        {
            return team?.Admins.Select(x => new TeamInfoStaffDto
            {
                Name = $"{x.FirstName} {x.LastName}",
                Position = x.Role.Name,
                Phones = x.Phone?.Select(ph => ph.PhoneNumber) ?? Array.Empty<string>(),
                EmailAddresses = x.Email?.Select(e => e.EmailAddress) ?? Array.Empty<string>(),
            }) ?? Array.Empty<TeamInfoStaffDto>();
        }

        private async Task<DateGroupedDto[]> GetScheduleAsync(string organizationId, string tournamentId, string teamId)
        {
            ProxyReference.Tournament tournament =
                await _tournamentProvider.GetTournamentAsync(organizationId, tournamentId);
            var venuesTask =
                _tournamentVenuesProvider.GetTournamentVenuesAsync(tournament.OrgKey, tournament.RecordKey);
            var teamGamesTask = GetTeamGamesAsync(tournament, teamId);

            var rounds = tournament.AgeGroups
                .Where(x => x.Flights != null)
                .SelectMany(x => x.Flights)
                .Where(x => x.Rounds != null)
                .SelectMany(x => x.Rounds);

            TournamentVenues venues = await venuesTask;
            var teamGames = await teamGamesTask;
            var playDtoItems = teamGames.Select(game => ScheduleHelper.GameToSchedulesMap(
                game,
                venues,
                rounds.First(round => round.Groups.Any(g => g.Key == game.RoundGroupKey)),
                _mapper));

            var groupedGames = playDtoItems.GroupBy(pDto => pDto.PlayDateTime.Date).Select(x => new DateGroupedDto
            {
                PlayDateTime = x.Key,
                SchedulesDto = x.ToList(),
            });

            return groupedGames.ToArray();
        }

        private async Task<TournamentGame[]> GetTeamGamesAsync(ProxyReference.Tournament tournament, string teamId)
        {
            var flightIds = tournament.AgeGroups
                .Where(ag => ag.Flights != null)
                .SelectMany(group => group.Flights)
                .Select(flight => new KeyValuePair<string, DateTime>(flight.Key, flight.LastUpdated))
                .Where(pair => !string.IsNullOrEmpty(pair.Key))
                .ToArray();

            var flightGames =
                (await _flightGamesProvider.GetFlightGamesListAsync(tournament.OrgKey, tournament.RecordKey, flightIds))
                .ToArray();

            return flightGames.SelectMany(x => x.Games)
                .Where(x => string.Equals(x.AwayTeamKey, teamId, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(x.HomeTeamKey, teamId, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }
    }
}
