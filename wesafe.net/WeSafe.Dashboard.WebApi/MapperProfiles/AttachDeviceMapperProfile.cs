using AutoMapper;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Dashboard.WebApi.Commands.Devices;

namespace WeSafe.Dashboard.WebApi.MapperProfiles
{
    public class AttachDeviceMapperProfile : Profile
    {
        public AttachDeviceMapperProfile()
        {
            CreateMap<IAttachDeviceContract, AttachDeviceCommand>();
        }
    }
}
