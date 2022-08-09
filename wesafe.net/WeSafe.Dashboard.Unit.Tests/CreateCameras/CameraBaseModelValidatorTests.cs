using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Dashboard.WebApi.Validators.Camera;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateCameras
{
    public class CameraBaseModelValidatorTests
    {
        private readonly CameraBaseValidator _validator;

        public CameraBaseModelValidatorTests()
        {
            _validator = new CameraBaseValidator();
        }

        [Fact]
        public async Task Validate_ModelIsNull_ThrowsBadRequestException()
        {
            await Assert.ThrowsAsync<BadRequestException>(async () => await _validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("somevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevaluesomevalue")]
        public async Task Validate_InvalidCameraName(string cameraName)
        {
            //arrange
            var model = new CameraBaseModel()
            {
                CameraName = cameraName
            };

            //act
            var result = await _validator.TestValidateAsync(model, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.CameraName);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalidvalueinvalidvalue")]

        public async Task Validate_InvalidIp(string ip)
        {
            //arrange
            var model = new CameraBaseModel()
            {
                Ip = ip
            };

            //act
            var result = await _validator.TestValidateAsync(model, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Ip);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalidvalueinvalidvalue")]

        public async Task Validate_InvalidPort(string port)
        {
            //arrange
            var model = new CameraBaseModel()
            {
                Port = port
            };

            //act
            var result = await _validator.TestValidateAsync(model, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Port);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalue")]

        public async Task Validate_InvalidLogin(string login)
        {
            //arrange
            var model = new CameraBaseModel()
            {
                Login = login
            };

            //act
            var result = await _validator.TestValidateAsync(model, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Login);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalueinvalidvalue")]

        public async Task Validate_InvalidPassword(string password)
        {
            //arrange
            var model = new CameraBaseModel()
            {
                Password = password
            };

            //act
            var result = await _validator.TestValidateAsync(model, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Password);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_ValidData()
        {
            //arrange
            var model = new CameraBaseModel()
            {
                CameraName = "Camera1",
                Login = "login",
                Password = "password",
                Ip = "1.1.1.1",
                Port = "80",
                SpecificRtcpConnectionString = "connectionString"
            };

            //act
            var result = await _validator.TestValidateAsync(model, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}
