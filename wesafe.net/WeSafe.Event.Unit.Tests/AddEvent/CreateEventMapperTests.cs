using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using WeSafe.Bus.Contracts.Event;
using WeSafe.Event.WebApi.Commands.AddEvent;
using WeSafe.Event.WebApi.MapperProfiles;
using Xunit;

namespace WeSafe.Event.Unit.Tests.AddEvent
{
    public class CreateEventMapperTests : AddEventBaseTest
    {
        private readonly MapperConfiguration _mapperConfiguration;

        public CreateEventMapperTests()
        {
            _mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<CreateEventMapper>());
        }

        [Fact]
        public void Map_ValidData_ModelIsMapped()
        {
            var mapper = _mapperConfiguration.CreateMapper();
            var model = GetValidCommand();

            var result = mapper.Map<ICreateEventContract>(model);

            Assert.NotNull(result);
            Assert.True(result.CameraIp == model.CameraIp);
            Assert.True(result.CameraId == model.CameraId);
            Assert.True(result.Alert == model.Alert);
            Assert.True(result.Message == model.Message);
            Assert.True(result.Frames.Count() == model.Frames.Count);
        }

        private AddEventCommand GetValidCommand()
        {
            return new AddEventCommand()
            {
                CameraId = 1,
                DeviceMacAddress = "f3:03:1c:39:13:b3",
                Frames = GetFileCollection(),
                CameraIp = "255.255.255.255",
                Alert = "SomeAlert",
                Message = "Some Message"
            };
        }
    }
}
