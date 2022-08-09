using AutoMapper;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.WebApi.Models;

namespace WeSafe.Logger.WebApi.MapperProfiles
{
    public class DeviceLogModelProfile : Profile
    {
        public DeviceLogModelProfile()
        {
            CreateMap<DeviceLogModelRequest, DeviceLogModel>();
        }
    }
}
