using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using WeSafe.Authentication.WebApi.Behaviors;
using WeSafe.Authentication.WebApi.Commands.Users;
using WeSafe.DAL.Entities;
using WeSafe.Web.Common.Authentication.Abstract;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.CreateUser
{
    public class CreateUserValidatorTests
    {
        private readonly Mock<IUserManager> _userManager;

        public CreateUserValidatorTests()
        {
            _userManager = new Mock<IUserManager>();
        }

        [Fact]
        public void Process_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateUserValidator(null));
        }

        [Fact]
        public async Task Process_UserFound_ThrowBadRequestException()
        {
            _userManager.Setup(c => c.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new User()).Verifiable();

            var validator = new CreateUserValidator(_userManager.Object);

            await Assert.ThrowsAsync<BadRequestException>(async () =>
                await validator.Process(new CreateUserCommand
                {
                    UserName = "user_name"
                }, CancellationToken.None));

            _userManager.Verify(c => c.FindByNameAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Process_ValidParameters_Success()
        {
            _userManager.Setup(c => c.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null).Verifiable();

            var validator = new CreateUserValidator(_userManager.Object);

            await validator.Process(new CreateUserCommand
            {
                UserName = "user_name"
            }, CancellationToken.None);

            _userManager.Verify(c => c.FindByNameAsync(It.IsAny<string>()), Times.Once);
        }
    }
}