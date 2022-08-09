using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Validators.Devices;
using WeSafe.Shared.Enumerations;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.UpdateDeviceType
{
    public class UpdateDeviceTypeCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new UpdateDeviceTypeCommandValidator();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Fact]
        public async Task Validate_InvalidDeviceIdValue()
        {
            //arrange
            var validator = new UpdateDeviceTypeCommandValidator();
            var command = new UpdateDeviceTypeCommand
            {
                DeviceId = 0
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.DeviceId);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_ValidData()
        {
            //arrange
            var validator = new UpdateDeviceTypeCommandValidator();
            var command = new UpdateDeviceTypeCommand
            {
                DeviceId = 1,
                DeviceType = DeviceType.PeopleRecognition
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}