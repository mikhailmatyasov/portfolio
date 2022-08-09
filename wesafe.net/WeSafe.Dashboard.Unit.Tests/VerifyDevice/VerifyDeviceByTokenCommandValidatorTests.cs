using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Validators.Devices;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.VerifyDevice
{
    public class VerifyDeviceByTokenCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new VerifyDeviceByTokenCommandValidator();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("2343434")]
        [InlineData("fgdgfgdf")]
        public async Task Validate_InvalidDeviceTokenValue(string token)
        {
            //arrange
            var validator = new VerifyDeviceByTokenCommandValidator();
            var command = new VerifyDeviceByTokenCommand
            {
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
            var validator = new VerifyDeviceByTokenCommandValidator();
            var command = new VerifyDeviceByTokenCommand
            {
                DeviceToken = "token123"
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}