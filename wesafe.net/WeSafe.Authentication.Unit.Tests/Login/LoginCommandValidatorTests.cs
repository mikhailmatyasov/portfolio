using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Authentication.WebApi.Commands.Login;
using WeSafe.Authentication.WebApi.Validators.Login;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.Login
{
    public class LoginCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new LoginCommandValidator();
            LoginCommand command = null;

            await Assert.ThrowsAsync<BadRequestException>(async () => await validator.TestValidateAsync(command, CancellationToken.None));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task Validate_InvalidNameValue(string name)
        {
            //arrange
            var validator = new LoginCommandValidator();
            LoginCommand command = new LoginCommand()
            {
                UserName = name,
                Password = "5899404"
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.UserName);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task Validate_InvalidPasswordValue(string password)
        {
            //arrange
            var validator = new LoginCommandValidator();
            LoginCommand command = new LoginCommand()
            {
                UserName = "Admin",
                Password = password
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Password);

            //assert
            Assert.True(errors.Any());
        }


        [Fact]
        public async Task Validate_NameIsTooLong()
        {
            //arrange
            var validator = new LoginCommandValidator();
            LoginCommand command = new LoginCommand()
            {
                UserName = new string('*', 51),
                Password = "5899404"
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.UserName);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_PasswordIsTooLong()
        {
            //arrange
            var validator = new LoginCommandValidator();
            LoginCommand command = new LoginCommand()
            {
                UserName = "Admin",
                Password = new string('*', 53),
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Password);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_PasswordIsTooShort()
        {
            //arrange
            var validator = new LoginCommandValidator();
            LoginCommand command = new LoginCommand()
            {
                UserName = "Admin",
                Password = new string('*', 3),
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Password);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData("user", "5899404")]
        [InlineData("___", "||||||")]
        [InlineData("עִבְרִית", "עִבְרִיתעִבְרִית")]
        public async Task Validate_ValidData(string username, string password)
        {
            //arrange
            var validator = new LoginCommandValidator();
            LoginCommand command = new LoginCommand()
            {
                UserName = username,
                Password = password,
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}
