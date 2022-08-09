using AutoMapper;
using System;
using WeSafe.Logger.Abstraction.Enums;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.MongoDbStorage.MapperProfiles;
using WeSafe.Logger.MongoDbStorage.Models;
using Xunit;

namespace WeSafe.Logger.Unit.Tests.AddLogs
{
    public class LogMapperProfileTests
    {
        private readonly MapperConfiguration _mapperConfiguration;

        public LogMapperProfileTests()
        {
            _mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<LogMapperProfile>());
        }

        [Fact]
        public void Map_ConvertInterfaceToDeviceLogBsonModel_TheInterfaceWasMapped()
        {
            var deviceLogModel = new DeviceLogModel()
            {
                CameraId = 1,
                CameraName = "CameraName",
                DateTime = new DateTime(2020, 7, 20),
                DeviceId = 1,
                DeviceName = "DeviceName",
                LogLevel = WeSafeLogLevel.Error,
                Message = "SomeMessage"
            };
            var mapper = _mapperConfiguration.CreateMapper();

            var result = mapper.Map<DeviceLogBsonModel>(deviceLogModel);

            Assert.NotNull(result);
            Assert.True(deviceLogModel.CameraId == result.CameraId);
            Assert.True(deviceLogModel.CameraName == result.CameraName);
            Assert.True(deviceLogModel.DateTime == result.DateTime);
            Assert.True(deviceLogModel.DeviceId == result.DeviceId);
            Assert.True(deviceLogModel.DeviceName == result.DeviceName);
            Assert.True(deviceLogModel.LogLevel == result.LogLevel);
            Assert.True(deviceLogModel.Message == result.Message);
        }
    }
}
