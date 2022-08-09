using AutoMapper;
using Model.Dto.Schedules;
using Model.Dto.VenueDetails;
using ProxyReference;
using ScheduleService.CacheEntities;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleService.Services.Common
{
    public static class ScheduleHelper
    {
        public static PlayDto GameToSchedulesMap(TournamentGame tournamentGame, TournamentVenues tournamentVenues, TournamentRound round, IMapper mapper)
        {
            TournamentVenue tournamentVenue = tournamentVenues.Venues
                .SingleOrDefault(tv => tv.Fields != null && tv.Fields.Any(tf => tf.RecordKey == tournamentGame.FieldKey));
            string[] teamGroups = tournamentGame.Description.Split(" vs ", System.StringSplitOptions.None);
            TournamentField field = tournamentVenue?.Fields.SingleOrDefault(tf => tf.RecordKey == tournamentGame.FieldKey);
            return new PlayDto
            {
                PlayDateTime = tournamentGame.StartTime,
                RoundName = round.Description,
                GameNumber = tournamentGame.GameNumber,
                HasBeenPlayed = tournamentGame.HasBeenPlayed,
                HasBeenScored = tournamentGame.HasBeenScored,
                HasOvertime = tournamentGame.Overtime,
                VenueShortName = tournamentVenue?.ShortName ?? string.Empty,
                VenueFullName = tournamentVenue?.Name ?? string.Empty,
                VenueAddresses = mapper.Map<IEnumerable<AddressDto>>(tournamentVenue?.Address),
                FieldNumber = field?.FieldNumber ?? string.Empty,
                HomeTeam = new TeamDto
                {
                    Id = tournamentGame.HomeTeam.TeamKey,
                    ClubId = tournamentGame.HomeTeam.ClubKey,
                    Name = tournamentGame.HomeTeam.TeamName,
                    Group = teamGroups[0],
                    TeamGroupCode = tournamentGame.HomeTeam.TeamGroupCode,
                    Goals = tournamentGame.HomeTeamGoals,
                    PenaltyKicks = tournamentGame.HomeTeamPKGoals,
                    IsForfeited = tournamentGame.HomeTeamForfeited,
                    IsOpponentForfeited = tournamentGame.AwayTeamForfeited,
                },
                AwayTeam = new TeamDto
                {
                    Id = tournamentGame.AwayTeam.TeamKey,
                    ClubId = tournamentGame.AwayTeam.ClubKey,
                    Name = tournamentGame.AwayTeam.TeamName,
                    Group = teamGroups[1],
                    TeamGroupCode = tournamentGame.AwayTeam.TeamGroupCode,
                    Goals = tournamentGame.AwayTeamGoals,
                    PenaltyKicks = tournamentGame.AwayTeamPKGoals,
                    IsForfeited = tournamentGame.AwayTeamForfeited,
                    IsOpponentForfeited = tournamentGame.HomeTeamForfeited,
                },
                Group = tournamentGame.Description,
                FlightKey = round.FlightKey,
                FieldKey = field?.RecordKey,
                VenueKey = field?.VenueKey,
            };
        }
    }
}
