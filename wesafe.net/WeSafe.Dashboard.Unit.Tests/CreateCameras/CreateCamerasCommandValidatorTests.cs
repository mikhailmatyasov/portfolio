using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Dashboard.WebApi.Validators.Camera;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateCameras
{
    public class CreateCamerasCommandValidatorTests
    {
        private const string _macAddress = "00:00:00:00";
        private readonly CreateCamerasCommandValidator _validator;

        public CreateCamerasCommandValidatorTests()
        {
            _validator = new CreateCamerasCommandValidator();
        }

        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            await Assert.ThrowsAsync<BadRequestException>(async () => await _validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Fact]
        public async Task Validate_InvalidMacAddressValue()
        {
            //arrange
            var command = new CreateCamerasCommand
            {
                Cameras = GetValidCameras()
            };

            //act
            var result = await _validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.MacAddress);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_NullCamerasValue()
        {
            //arrange
            var command = new CreateCamerasCommand
            {
                MacAddress = _macAddress
            };

            //act
            var result = await _validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Cameras);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_InvalidCamerasValue()
        {
            //arrange
            var command = new CreateCamerasCommand
            {
                MacAddress = _macAddress,
                Cameras = GetInvalidCameras().ToArray()
            };
            
            //act
            var result = await _validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.Errors;
            
            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_ValidData()
        {
            //arrange
            var command = new CreateCamerasCommand
            {
                MacAddress = _macAddress,
                Cameras = GetValidCameras()
            };

            //act
            var result = await _validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }

        private IEnumerable<CameraBaseModel> GetValidCameras()
        {
            return new CameraBaseModel[]
            {
                new CameraBaseModel()
                {
                    CameraName = "Camera1",
                    Login = "login",
                    Password = "password",
                    Ip = "1.1.1.1",
                    Port = "80",
                    SpecificRtcpConnectionString = "connectionString"
                },
                new CameraBaseModel()
                {
                    CameraName = "Camera2",
                    Login = "login",
                    Password = "password",
                    Ip = "2.2.2.2",
                    Port = "80",
                    SpecificRtcpConnectionString = "connectionString"
                }
            };
        }

        private IEnumerable<CameraBaseModel> GetInvalidCameras()
        {
            return new CameraBaseModel[]
            {
                new CameraBaseModel()
                {
                    CameraName = string.Empty,
                    Login = string.Empty,
                    Password = string.Empty,
                    Ip = string.Empty,
                    Port = string.Empty,
                    SpecificRtcpConnectionString = string.Empty
                }
            };
        }
    }
}
