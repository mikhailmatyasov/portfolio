using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.Authentication.WebApi.Behaviors;
using WeSafe.Authentication.WebApi.Commands.MobileUser;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.DAL.Abstractions;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.MobileUser
{
    public class CreateMobileUserValidatorTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMobileUserRepository> _repository;
        private readonly CreateMobileUserCommand _createMobileUserCommand;

        public CreateMobileUserValidatorTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IMobileUserRepository>();

            _unitOfWork.Setup(c => c.GetRepository<DAL.Entities.MobileUser>(true)).Returns(_repository.Object);

            _createMobileUserCommand = new CreateMobileUserCommand
            {
                PhoneNumber = "+71234567890",
                Status = MobileUserStatus.Active
            };
        }

        [Fact]
        public void Process_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateMobileUserValidator(null));
        }

        [Fact]
        public async Task Process_UserExists_ThrowsInvalidOperationException()
        {
            // arrange
            _repository.Setup(x => x.FindByPhoneNumberAsync(It.IsAny<string>(), true))
                       .ReturnsAsync(() => new DAL.Entities.MobileUser())
                       .Verifiable();

            var validator = new CreateMobileUserValidator(_unitOfWork.Object);

            // assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await validator.Process(_createMobileUserCommand, CancellationToken.None));

            _repository.Verify(c => c.FindByPhoneNumberAsync(It.IsAny<string>(), true), Times.Once);
        }

        [Fact]
        public async Task Process_UserNotExists_Success()
        {
            // arrange
            _repository.Setup(x => x.FindByPhoneNumberAsync(It.IsAny<string>(), true))
                       .ReturnsAsync(() => null)
                       .Verifiable();

            var validator = new CreateMobileUserValidator(_unitOfWork.Object);

            await validator.Process(_createMobileUserCommand, CancellationToken.None);

            _repository.Verify(c => c.FindByPhoneNumberAsync(It.IsAny<string>(), true), Times.Once);
        }
    }
}