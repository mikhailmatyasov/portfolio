using AutoMapper;
using Model.Dto.Venues;
using ProxyReference;

namespace ScheduleService.Mapping
{
    public class VenuesMappingProfile : Profile
    {
        public VenuesMappingProfile()
        {
            CreateMap<TournamentFlight, VenuesFlightDto>();
            CreateMap<TournamentRound, VenuesRoundDto>();
            CreateMap<TournamentVenue, VenuesVenueDto>();
        }
    }
}
