using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Authentication.WebApi.Commands.DeviceLogin;
using WeSafe.Authentication.WebApi.Validators.Device;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.DeviceLogin
{
    public class DeviceLoginCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new DeviceLoginCommandValidator();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some value")]
        [InlineData("00:30:48:5R:58:65")]
        public async Task Validate_InvalidDeviceValue(string device)
        {
            //arrange
            var validator = new DeviceLoginCommandValidator();
            var command = new DeviceLoginCommand()
            {
                MacAddress = device,
                Secret = null
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.MacAddress);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData("00:30:48:5a:58:65")]
        [InlineData("00:30:48:5A:58:65")]
        public async Task Validate_ValidData(string device)
        {
            //arrange
            var validator = new DeviceLoginCommandValidator();
            var command = new DeviceLoginCommand()
            {
                MacAddress = device,
                Secret = null
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}