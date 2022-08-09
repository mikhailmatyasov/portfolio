using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.Abstraction.Services;
using WeSafe.Logger.WebApi.Services;
using Xunit;

namespace WeSafe.Logger.Unit.Tests.AddLogs
{
    public class CameraNameLogMapperTests
    {
        private readonly Mock<ICameraService> _mockCameraService;
        private readonly CameraNameLogMapper _mapper;

        public CameraNameLogMapperTests()
        {
            _mockCameraService = new Mock<ICameraService>();
            _mockCameraService.Setup(x => x.GetCamerasNames(It.IsAny<CamerasNamesRequest>())).ReturnsAsync(
                new List<CameraName>()
                {
                    new CameraName()
                    {
                        Id = 1,
                        Name = "Camera"
                    },

                    new CameraName()
                    {
                        Id = 2,
                        Name = "Camera2"
                    }
                });

            _mapper = new CameraNameLogMapper(_mockCameraService.Object);
        }

        [Fact]
        public async Task Map_LogsIsNull_ThrowArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _mapper.Map(null));
        }

        [Fact]
        public async Task Map_ValidLogs_LogCameraNameWasMapped()
        {
            var deviceLog = new DeviceLogModel()
            {
                CameraId = 1
            };

            var logs = new List<DeviceLogModel>()
            {
                deviceLog
            };

            await _mapper.Map(logs);

            Assert.NotNull(deviceLog);
            Assert.True(deviceLog.CameraName == "Camera");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(null)]
        public async Task Map_CameraIdIsEmpty_LonCameraNameWasNotMapped(int? cameraId)
        {
            var deviceLog = new DeviceLogModel()
            {
                CameraId = cameraId
            };

            var logs = new List<DeviceLogModel>()
            {
                deviceLog
            };

            await _mapper.Map(logs);

            Assert.NotNull(deviceLog);
            Assert.Null(deviceLog.CameraName);
        }

        [Fact]
        public async Task Map_OneOfLogIsNull_NoErrorsLogCameraNameWasMapped()
        {
            var logs = new List<DeviceLogModel>()
            {
                null
            };

            await _mapper.Map(logs);
        }
    }
}
