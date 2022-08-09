using System;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.Abstraction.Services;
using WeSafe.Logger.WebApi.Services;
using Xunit;

namespace WeSafe.Logger.Unit.Tests.AddLogs
{
    public class DeviceNameLogMapperTests
    {
        private readonly Mock<IDeviceService> _mockDeviceService;
        private readonly DeviceNameLogMapper _mapper;

        public DeviceNameLogMapperTests()
        {
            _mockDeviceService = new Mock<IDeviceService>();
            _mockDeviceService.Setup(x => x.GetDevicesNames(It.IsAny<DevicesNamesRequest>())).ReturnsAsync(
                new List<DeviceName>()
                {
                    new DeviceName()
                    {
                        Id = 1,
                        Name = "Device"
                    },

                    new DeviceName()
                    {
                        Id = 2,
                        Name = "Device2"
                    }
                });

            _mapper = new DeviceNameLogMapper(_mockDeviceService.Object);
        }

        [Fact]
        public async Task Map_LogsIsNull_ThrowArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _mapper.Map(null));
        }

        [Fact]
        public async Task Map_ValidLogs_LogDeviceNameWasMapped()
        {
            var deviceLog = new DeviceLogModel()
            {
                DeviceId = 1
            };

            var logs = new List<DeviceLogModel>()
            {
                deviceLog
            };

            await _mapper.Map(logs);

            Assert.NotNull(deviceLog);
            Assert.True(deviceLog.DeviceName == "Device");
        }

        [Fact]
        public async Task Map_DeviceIdIsInvalid_LogNameWasNotMapped()
        {
            var deviceLog = new DeviceLogModel()
            {
                CameraId = -1
            };

            var logs = new List<DeviceLogModel>()
            {
                deviceLog
            };

            await _mapper.Map(logs);

            Assert.NotNull(deviceLog);
            Assert.Null(deviceLog.DeviceName);
        }

        [Fact]
        public async Task Map_OneOfLogIsNull_NoErrorsLogDeviceNameWasMapped()
        {
            var logs = new List<DeviceLogModel>()
            {
                null
            };

            await _mapper.Map(logs);
        }
    }
}
