using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.Authentication.WebApi.Behaviors;
using WeSafe.Authentication.WebApi.Commands.MobileLogin;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.MobileLogin
{
    public class MobileValidatorTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ISubscriberRepository> _repository;
        private readonly MobileLoginCommand _mobileLoginCommand;

        public MobileValidatorTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<ISubscriberRepository>();

            _unitOfWork.Setup(c => c.GetRepository<ClientSubscriber>(true)).Returns(_repository.Object);

            _mobileLoginCommand = new MobileLoginCommand
            {
                PhoneNumber = "+71234567890"
            };
        }

        [Fact]
        public void Process_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MobileValidator(null));
        }

        [Fact]
        public async Task Process_ActiveClientsNotFound_ThrowsUnauthorizedException()
        {
            // arrange
            _repository.Setup(x => x.HasActiveClientsAsync(It.IsAny<string>())).ReturnsAsync(() => false).Verifiable();

            var mobileValidator = new MobileValidator(_unitOfWork.Object);

            // assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () =>
                await mobileValidator.Process(_mobileLoginCommand, CancellationToken.None));

            _repository.Verify(c => c.HasActiveClientsAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Process_DeviceFound_Success()
        {
            // arrange
            _repository.Setup(x => x.HasActiveClientsAsync(It.IsAny<string>())).ReturnsAsync(() => true).Verifiable();

            var mobileValidator = new MobileValidator(_unitOfWork.Object);

            await mobileValidator.Process(_mobileLoginCommand, CancellationToken.None);

            _repository.Verify(c => c.HasActiveClientsAsync(It.IsAny<string>()), Times.Once);
        }
    }
}