using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Authentication.WebApi.Commands.MobileLogin;
using WeSafe.Authentication.WebApi.Validators.Mobile;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.MobileLogin
{
    public class MobileLoginCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new MobileLoginCommandValidator();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some value")]
        [InlineData("+764555434434554")]
        public async Task Validate_InvalidPhoneValue(string phone)
        {
            //arrange
            var validator = new MobileLoginCommandValidator();
            var command = new MobileLoginCommand()
            {
                PhoneNumber = phone
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData("+71234567883")]
        [InlineData("+712345678835")]
        public async Task Validate_ValidData(string phone)
        {
            //arrange
            var validator = new MobileLoginCommandValidator();
            var command = new MobileLoginCommand()
            {
                PhoneNumber = phone
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}