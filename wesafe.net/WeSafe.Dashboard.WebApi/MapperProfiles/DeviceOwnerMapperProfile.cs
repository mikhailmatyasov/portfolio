using AutoMapper;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.MapperProfiles
{
    public class DeviceOwnerMapperProfile : Profile
    {
        public DeviceOwnerMapperProfile()
        {
            CreateMap<ICreateDeviceOwnerContract, ClientModel>();
        }
    }
}