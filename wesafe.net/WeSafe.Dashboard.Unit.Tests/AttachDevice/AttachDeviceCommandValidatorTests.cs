using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Validators.Devices;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.AttachDevice
{
    public class AttachDeviceCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new AttachDeviceCommandValidator();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Fact]
        public async Task Validate_InvalidClientIdValue()
        {
            //arrange
            var validator = new AttachDeviceCommandValidator();
            var command = new AttachDeviceCommand
            {
                ClientId = 0,
                DeviceToken = "2token"
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.ClientId);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("2343434")]
        [InlineData("fgdgfgdf")]
        public async Task Validate_InvalidDeviceTokenValue(string token)
        {
            //arrange
            var validator = new AttachDeviceCommandValidator();
            var command = new AttachDeviceCommand
            {
                ClientId = 1,
                DeviceToken = token
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.DeviceToken);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_ValidData()
        {
            //arrange
            var validator = new AttachDeviceCommandValidator();
            var command = new AttachDeviceCommand
            {
                ClientId = 1,
                DeviceToken = "token123"
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}