using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Event.WebApi.Commands.AddEvent;
using WeSafe.Event.WebApi.Validators;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Event.Unit.Tests.AddEvent
{
    public class AddEventCommandValidatorTests : AddEventBaseTest
    {
        private readonly AddEventCommandValidator _addEventCommandValidator;

        public AddEventCommandValidatorTests()
        {
            _addEventCommandValidator = new AddEventCommandValidator();
        }

        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            await Assert.ThrowsAsync<BadRequestException>(async () => await _addEventCommandValidator.TestValidateAsync(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Validate_InvalidCameraId_ModelIsInvalid(int cameraId)
        {
            var command = GetValidCommand();
            command.CameraId = cameraId;

            var result = await _addEventCommandValidator.TestValidateAsync(command, CancellationToken.None);

            Assert.True(result.ShouldHaveValidationErrorFor(x => x.CameraId).Any());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("    ")]
        [InlineData("ZZ:03:1c:39:13:b3")]
        [InlineData("59-c8-ea-91-60-cd")]
        public async Task Validate_InvalidMacAddress_ModelIsInvalid(string macAddress)
        {
            var command = GetValidCommand();
            command.DeviceMacAddress = macAddress;

            var result = await _addEventCommandValidator.TestValidateAsync(command, CancellationToken.None);

            Assert.True(result.ShouldHaveValidationErrorFor(x => x.DeviceMacAddress).Any());
        }

        [Fact]
        public async Task Validate_FramesIsNull_ModelIsInvalid()
        {
            var command = GetValidCommand();
            command.Frames = null;

            var result = await _addEventCommandValidator.TestValidateAsync(command, CancellationToken.None);

            Assert.True(result.ShouldHaveValidationErrorFor(x => x.Frames).Any());
        }

        [Fact]
        public async Task Validate_FramesCollectionIssEmpty_ModelIsInvalid()
        {
            var command = GetValidCommand();
            command.Frames = new FormFileCollection();

            var result = await _addEventCommandValidator.TestValidateAsync(command, CancellationToken.None);

            Assert.True(result.ShouldHaveValidationErrorFor(x => x.Frames).Any());
        }

        [Theory]
        [InlineData("700.122.12.55")]
        [InlineData("255-0.0.0")]
        [InlineData("255.0.0.-1")]
        public async Task Validate_InvalidCameraIp_ModelIsInvalid(string cameraIp)
        {
            var command = GetValidCommand();
            command.CameraIp = cameraIp;

            var result = await _addEventCommandValidator.TestValidateAsync(command, CancellationToken.None);

            Assert.True(result.ShouldHaveValidationErrorFor(x => x.CameraIp).Any());
        }

        [Fact]
        public async Task Validate_ValidData_ModelIsValid()
        {
            var command = GetValidCommand();

            var result = await _addEventCommandValidator.TestValidateAsync(command, CancellationToken.None);

            Assert.True(result.IsValid);
        }

        private AddEventCommand GetValidCommand()
        {
            return new AddEventCommand()
            {
                CameraId = 1,
                DeviceMacAddress = "f3:03:1c:39:13:b3",
                Frames = GetFileCollection()
            };
        }
    }
}
