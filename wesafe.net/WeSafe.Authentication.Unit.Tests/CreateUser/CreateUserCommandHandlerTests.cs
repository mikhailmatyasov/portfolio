using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using WeSafe.Authentication.WebApi.Commands.Users;
using WeSafe.DAL.Entities;
using WeSafe.Web.Common.Authentication.Abstract;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.CreateUser
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUserManager> _userManager;

        public CreateUserCommandHandlerTests()
        {
            _userManager = new Mock<IUserManager>();
        }

        [Fact]
        public void Process_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateUserCommandHandler(null));
        }

        [Fact]
        public async Task Process_CreateUser_Success()
        {
            string userId = Guid.NewGuid().ToString();

            _userManager.Setup(c => c.CreateAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(userId)
                        .Verifiable();

            var handler = new CreateUserCommandHandler(_userManager.Object);

            var result = await handler.Handle(new CreateUserCommand(), CancellationToken.None);

            Assert.Equal(userId, result);
            _userManager.Verify(c => c.CreateAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}