using AutoMapper;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.MapperProfiles.Converters;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.MapperProfiles
{
    public class SubscriberMapperProfile : Profile
    {
        public SubscriberMapperProfile()
        {
            CreateMap<ClientSubscriber, SubscriberModel>()
                .ForMember(dest => dest.Permissions, src => src.ConvertUsing(new SubscriberPermissionConverter()));
        }
    }
}