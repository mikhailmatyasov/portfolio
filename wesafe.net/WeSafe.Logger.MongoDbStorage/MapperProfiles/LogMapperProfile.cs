using AutoMapper;
using WeSafe.Logger.Abstraction.Interfaces;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.MongoDbStorage.Models;

namespace WeSafe.Logger.MongoDbStorage.MapperProfiles
{
    public class LogMapperProfile : Profile
    {
        public LogMapperProfile()
        {
            CreateMap<IWeSafeLog, DeviceLogBsonModel>()
                .Include<DeviceLogModel, DeviceLogBsonModel>();

            CreateMap<DeviceLogModel, DeviceLogBsonModel>();
        }
    }
}
