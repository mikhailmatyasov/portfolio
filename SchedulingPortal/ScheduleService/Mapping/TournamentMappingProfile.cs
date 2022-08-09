using AutoMapper;
using Model.Dto.Common;
using Model.Dto.Tournament;
using ProxyReference;

namespace ScheduleService.Mapping
{
    public class TournamentMappingProfile : Profile
    {
        public TournamentMappingProfile()
        {
            CreateMap<TournamentFlight, TournamentFlightDto>();
            CreateMap<TournamentInfo, TournamentInfoDto>();
            CreateMap<Tournament, TournamentDto>();
            CreateMap<Tournament, PageHeaderDto>();
        }
    }
}
