using AutoMapper;
using Model.Dto.Schedules;
using Model.Dto.Standings;
using ProxyReference;

namespace ScheduleService.Mapping
{
    public class StandingsMappingProfile : Profile
    {
        public StandingsMappingProfile()
        {
            CreateMap<TournamentStanding, TeamStandingsDto>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(x => x.TeamKey))
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(x => x.TeamName))
                .ForMember(
                    dest => dest.Points,
                    opt => opt.MapFrom(x => x.TotalScore))
                .ForMember(
                    dest => dest.GamesPlayed,
                    opt => opt.MapFrom(x => x.TotalGames))
                .ForMember(
                    dest => dest.GoalsFor,
                    opt => opt.MapFrom(x => x.GoalsScoredFor))
                .ForMember(
                    dest => dest.GoalsAgainst,
                    opt => opt.MapFrom(x => x.GoalsScoredAgainst));

            CreateMap<TournamentStanding, StandingsBracketDto>()
                .ForMember(
                    dest => dest.GamesPlayed,
                    opt => opt.MapFrom(x => x.TotalGames))
                .ForMember(
                    dest => dest.GoalsFor,
                    opt => opt.MapFrom(x => x.GoalsScoredFor))
                .ForMember(
                    dest => dest.GoalsAgainst,
                    opt => opt.MapFrom(x => x.GoalsScoredAgainst));
        }
    }
}
