using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.WebApi.Services;
using Xunit;

namespace WeSafe.Logger.Unit.Tests.AddLogs
{
    public class DeviceLogFilterTests
    {
        private readonly DeviceLogFilter _deviceLogFilter;

        public DeviceLogFilterTests()
        {
            _deviceLogFilter = new DeviceLogFilter();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        public async Task Filter_ValidLogs_FilteredLogsThatIsNullAndDeviceNameIsEmpty(string deviceName)
        {
            var logs = new List<DeviceLogModel>()
            {
                new DeviceLogModel()
                {
                    DeviceName = "SomeDeviceName"
                },
                null,
                new DeviceLogModel()
                {
                    DeviceName = deviceName
                }
            };

            var result = await _deviceLogFilter.Filter(logs);

            Assert.NotNull(result);
            Assert.True(result.Count() == 1);
        }
    }
}
