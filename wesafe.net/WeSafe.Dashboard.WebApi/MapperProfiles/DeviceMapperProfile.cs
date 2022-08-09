using AutoMapper;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.MapperProfiles
{
    public class DeviceMapperProfile : Profile
    {
        public DeviceMapperProfile()
        {
            CreateMap<Device, DeviceModel>().ReverseMap();
        }
    }
}