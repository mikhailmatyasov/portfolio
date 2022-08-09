using System;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.Authentication.WebApi.Commands.MobileUser;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.DAL.Abstractions;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.MobileUser
{
    public class CreateMobileUserCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMobileUserRepository> _repository;

        public CreateMobileUserCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IMobileUserRepository>();

            _unitOfWork.Setup(c => c.GetRepository<DAL.Entities.MobileUser>(true)).Returns(_repository.Object);
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateMobileUserCommandHandler(null));
        }

        [Fact]
        public async Task Process_CreateUser_Success()
        {
            _repository.Setup(c => c.Insert(It.IsAny<DAL.Entities.MobileUser>())).Verifiable();
            _unitOfWork.Setup(c => c.SaveChangesAsync(false)).Verifiable();

            var handler = new CreateMobileUserCommandHandler(_unitOfWork.Object);

            await handler.Handle(new CreateMobileUserCommand
            {
                PhoneNumber = "+71234567890",
                Status = MobileUserStatus.Active
            }, default);

            _repository.Verify(c => c.Insert(It.IsAny<DAL.Entities.MobileUser>()), Times.Once);
            _unitOfWork.Verify(c => c.SaveChangesAsync(false), Times.Once);
        }
    }
}