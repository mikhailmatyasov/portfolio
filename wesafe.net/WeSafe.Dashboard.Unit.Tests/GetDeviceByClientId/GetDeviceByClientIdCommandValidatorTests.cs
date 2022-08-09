using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Validators.Devices;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.GetDeviceByClientId
{
    public class GetDeviceByClientIdCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new GetDeviceByClientIdCommandValidator();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task Validate_InvalidClientIdValue(int clientId)
        {
            //arrange
            var validator = new GetDeviceByClientIdCommandValidator();
            var command = new GetDeviceByClientIdCommand
            {
                ClientId = clientId,
                DeviceId = 1
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.ClientId);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task Validate_InvalidDeviceIdValue(int deviceId)
        {
            //arrange
            var validator = new GetDeviceByClientIdCommandValidator();
            var command = new GetDeviceByClientIdCommand
            {
                ClientId = 1,
                DeviceId = deviceId
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.DeviceId);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_InvalidData()
        {
            //arrange
            var validator = new GetDeviceByClientIdCommandValidator();
            var command = new GetDeviceByClientIdCommand
            {
                ClientId = -1,
                DeviceId = -5
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.DeviceId);
            var errors1 = result.ShouldHaveValidationErrorFor(x => x.ClientId);

            //assert
            Assert.True(errors.Any() && errors1.Any());
        }

        [Fact]
        public async Task Validate_ValidData()
        {
            //arrange
            var validator = new GetDeviceByClientIdCommandValidator();
            var command = new GetDeviceByClientIdCommand
            {
                ClientId = 1,
                DeviceId = 5
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}