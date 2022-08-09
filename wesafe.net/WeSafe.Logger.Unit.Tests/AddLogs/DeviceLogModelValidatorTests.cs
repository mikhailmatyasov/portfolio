using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.WebApi.Validators.LogValidators;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Logger.Unit.Tests.AddLogs
{
    public class DeviceLogModelValidatorTests
    {
        private readonly DeviceLogModelValidator _deviceLogModelValidator;

        public DeviceLogModelValidatorTests()
        {
            _deviceLogModelValidator = new DeviceLogModelValidator();
        }

        [Fact]
        public async Task Validate_ModelIsNull_ThrowsBadRequestException()
        {
            await Assert.ThrowsAsync<BadRequestException>(async () => await _deviceLogModelValidator.TestValidateAsync(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        public async Task Validate_MessageIsNull_InvalidLogMessage(string message)
        {
            var log = GetValidLogModel();
            log.Message = message;

            var result = await _deviceLogModelValidator.TestValidateAsync(log, CancellationToken.None);

            Assert.True(result.ShouldHaveValidationErrorFor(x => x.Message).Any());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Validate_IncorrectDeviceId_InvalidDeviceIdentifier(int deviceId)
        {
            var log = GetValidLogModel();
            log.DeviceId = deviceId;

            var result = await _deviceLogModelValidator.TestValidateAsync(log, CancellationToken.None);

            Assert.True(result.ShouldHaveValidationErrorFor(x => x.DeviceId).Any());
        }

        [Fact]
        public async Task Validate_IncorrectDateTime_InvalidLogDateTime()
        {
            var log = GetValidLogModel();
            log.DateTime = default;

            var result = await _deviceLogModelValidator.TestValidateAsync(log, CancellationToken.None);

            Assert.True(result.ShouldHaveValidationErrorFor(x => x.DateTime).Any());
        }

        [Fact]
        public async Task Validate_ValidData_NoErrors()
        {
            var log = GetValidLogModel();

            var result = await _deviceLogModelValidator.TestValidateAsync(log, CancellationToken.None);

            Assert.True(result.IsValid);
        }


        private DeviceLogModel GetValidLogModel() => new DeviceLogModel()
        {
            DeviceId = 1,
            DateTime = DateTime.Now,
            Message = "SomeMessage"
        };
    }
}
