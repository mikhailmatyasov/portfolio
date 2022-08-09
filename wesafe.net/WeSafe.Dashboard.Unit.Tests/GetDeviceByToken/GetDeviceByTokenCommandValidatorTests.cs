using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Validators.Devices;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.GetDeviceByToken
{
    public class GetDeviceByTokenCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new GetDeviceByTokenCommandValidator();

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
            var validator = new GetDeviceByTokenCommandValidator();
            var command = new GetDeviceByTokenCommand
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
            var validator = new GetDeviceByTokenCommandValidator();
            var command = new GetDeviceByTokenCommand
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