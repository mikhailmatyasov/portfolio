using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using WeSafe.Authentication.WebApi.Behaviors;
using WeSafe.Authentication.WebApi.Commands.Login;
using WeSafe.DAL.Entities;
using WeSafe.Web.Common.Authentication.Abstract;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.Login
{
    public class UserValidatorTests
    {
        private readonly Mock<IUserManager> _mockUserManager;
        private readonly LoginCommand _loginCommand;

        public UserValidatorTests()
        {
            _mockUserManager = new Mock<IUserManager>();
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new User()
            {
                IsActive = true
            });

            _mockUserManager.Setup(x => x.IsAdmin(It.IsAny<User>())).ReturnsAsync(true);

            _mockUserManager
                .Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            _loginCommand = new LoginCommand()
            {
                UserName = "someUserName",
                Password = "somePassword",
                Origin = "Origin"
            };
        }

        [Fact]
        public void Process_UserManagerIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new UserValidator(null));
        }

        [Fact]
        public async Task Process_UserNotFound_ThrowsUnauthorizedException()
        {
            // arrange
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(() => null);
            var userValidator = new UserValidator(_mockUserManager.Object);

            // assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await userValidator.Process(_loginCommand, CancellationToken.None));
        }

        [Fact]
        public async Task Process_UserIsNotActive_ThrowsUnauthorizedException()
        {
            // arrange
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new User() { IsActive = false });
            var userValidator = new UserValidator(_mockUserManager.Object);

            // assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await userValidator.Process(_loginCommand, CancellationToken.None));
        }

        [Fact]
        public async Task Process_InvalidPassword_ThrowsUnauthorizedException()
        {
            // arrange
            _mockUserManager
                .Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            var userValidator = new UserValidator(_mockUserManager.Object);

            // assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await userValidator.Process(_loginCommand, CancellationToken.None));
        }

        [Fact]
        public async Task Process_UserIsLocked_ThrowsUnauthorizedException()
        {
            // arrange
            _mockUserManager
                .Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.LockedOut);

            var userValidator = new UserValidator(_mockUserManager.Object);

            // assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await userValidator.Process(_loginCommand, CancellationToken.None));
        }
    }
}
