using System;
using System.Threading.Tasks;
using Moq;
using WeSafe.Authentication.WebApi.Commands.VerifyLogin;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.DAL.Entities;
using WeSafe.Web.Common.Authentication.Abstract;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.VerifyLogin
{
    public class VerifyLoginCommandHandlerTests
    {
        private readonly Mock<IUserManager> _userManager;

        public VerifyLoginCommandHandlerTests()
        {
            _userManager = new Mock<IUserManager>();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new VerifyLoginCommandHandler(null));
        }

        [Fact]
        public async Task Process_LoginFound_ReturnExists()
        {
            _userManager.Setup(c => c.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(() => new User()).Verifiable();

            var handler = new VerifyLoginCommandHandler(_userManager.Object);

            var status = await handler.Handle(new VerifyLoginCommand
            {
                UserName = "user1"
            }, default);

            Assert.Equal(LoginStatus.Exists, status);

            _userManager.Verify(c => c.FindByNameAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Process_NotFound_ReturnOk()
        {
            _userManager.Setup(c => c.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(() => null).Verifiable();

            var handler = new VerifyLoginCommandHandler(_userManager.Object);

            var status = await handler.Handle(new VerifyLoginCommand
            {
                UserName = "user1"
            }, default);

            Assert.Equal(LoginStatus.Ok, status);

            _userManager.Verify(c => c.FindByNameAsync(It.IsAny<string>()), Times.Once);
        }
    }
}