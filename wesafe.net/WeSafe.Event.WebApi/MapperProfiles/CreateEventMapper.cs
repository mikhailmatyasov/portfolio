using AutoMapper;
using WeSafe.Bus.Components.Models.Event;
using WeSafe.Bus.Contracts.Event;
using WeSafe.Event.WebApi.Commands.AddEvent;
using WeSafe.Event.WebApi.MapperProfiles.Resolvers;

namespace WeSafe.Event.WebApi.MapperProfiles
{
    public class CreateEventMapper : Profile
    {
        public CreateEventMapper()
        {
            CreateMap<AddEventCommand, ICreateEventContract>()
                .ForMember(x => x.Frames,
                    opt => opt.MapFrom<FormFileCollectionResolver>()).As<CreateEventContract>();
        }
    }
}