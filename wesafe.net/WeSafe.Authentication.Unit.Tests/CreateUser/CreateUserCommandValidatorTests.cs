using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Authentication.WebApi.Commands.Users;
using WeSafe.Authentication.WebApi.Validators.Users;
using WeSafe.Shared.Roles;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.CreateUser
{
    public class CreateUserCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new CreateUserCommandValidator();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Very large text. Greater than 50 characters. It is not good.")]
        public async Task Validate_InvalidUserNameValue(string userName)
        {
            //arrange
            var validator = new CreateUserCommandValidator();
            var command = new CreateUserCommand
            {
                UserName = userName,
                Phone = "+71234567890",
                DisplayName = "name",
                RoleName = UserRoles.Users,
                Password = "123456"
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.UserName);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData("some value")]
        [InlineData("+764555434434554")]
        public async Task Validate_InvalidPhoneValue(string phone)
        {
            //arrange
            var validator = new CreateUserCommandValidator();
            var command = new CreateUserCommand()
            {
                UserName = "username",
                Phone = phone,
                DisplayName = "name",
                RoleName = UserRoles.Users,
                Password = "123456"
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Phone);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Very large text. Greater than 250 characters. It is not good. 11111111111111111111111111111344444444444444444444444444444444444444111111111111111111111111111188888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888888")]
        public async Task Validate_InvalidDisplayNameValue(string displayName)
        {
            //arrange
            var validator = new CreateUserCommandValidator();
            var command = new CreateUserCommand()
            {
                UserName = "username",
                Phone = "+71234567890",
                DisplayName = displayName,
                RoleName = UserRoles.Users,
                Password = "123456"
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.DisplayName);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("bad_role_name")]
        public async Task Validate_InvalidRoleNameValue(string roleName)
        {
            //arrange
            var validator = new CreateUserCommandValidator();
            var command = new CreateUserCommand
            {
                UserName = "userName",
                Phone = "+71234567890",
                DisplayName = "name",
                RoleName = roleName,
                Password = "123456"
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.RoleName);

            //assert
            Assert.True(errors.Any());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("Very large password. Greater than 50 characters. It is not good.")]
        public async Task Validate_InvalidPasswordValue(string password)
        {
            //arrange
            var validator = new CreateUserCommandValidator();
            var command = new CreateUserCommand
            {
                UserName = "userName",
                Phone = "+71234567890",
                DisplayName = "name",
                RoleName = UserRoles.Users,
                Password = password
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Password);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_ValidData()
        {
            //arrange
            var validator = new CreateUserCommandValidator();
            var command = new CreateUserCommand
            {
                UserName = "userName",
                Phone = "+71234567890",
                DisplayName = "name",
                RoleName = UserRoles.Users,
                Password = "1234567"
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}