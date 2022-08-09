using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Authentication.WebApi.Commands.VerifyLogin;
using WeSafe.Authentication.WebApi.Validators.VerifyLogin;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.VerifyLogin
{
    public class VerifyLoginCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new VerifyLoginCommandValidator();
            VerifyLoginCommand command = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(command, CancellationToken.None));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task Validate_InvalidNameValue(string name)
        {
            //arrange
            var validator = new VerifyLoginCommandValidator();
            var command = new VerifyLoginCommand()
            {
                UserName = name
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.UserName);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_NameIsTooLong()
        {
            //arrange
            var validator = new VerifyLoginCommandValidator();
            var command = new VerifyLoginCommand()
            {
                UserName = new string('*', 51)
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.UserName);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData("user")]
        [InlineData("___")]
        [InlineData("עִבְרִית")]
        public async Task Validate_ValidData(string username)
        {
            //arrange
            var validator = new VerifyLoginCommandValidator();
            var command = new VerifyLoginCommand()
            {
                UserName = username
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}
