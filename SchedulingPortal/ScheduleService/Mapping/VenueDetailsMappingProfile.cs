using AutoMapper;
using Model.Dto.VenueDetails;
using ProxyReference;

namespace ScheduleService.Mapping
{
    public class VenueDetailsMappingProfile : Profile
    {
        public VenueDetailsMappingProfile()
        {
            CreateMap<Address, AddressDto>();
            CreateMap<FieldClosure, FieldClosureDto>();
            CreateMap<TournamentField, FieldDto>()
                .ForMember(dest => dest.FieldClosures, opt => opt.MapFrom(x => x.Closures));
            CreateMap<TournamentVenue, VenueDetailsDto>();
        }
    }
}
