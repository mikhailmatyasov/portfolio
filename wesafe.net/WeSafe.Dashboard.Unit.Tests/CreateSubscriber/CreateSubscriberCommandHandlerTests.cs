using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Commands.Subscribers;
using WeSafe.Dashboard.WebApi.Enumerations;
using WeSafe.Dashboard.WebApi.Models;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateSubscriber
{
    public class CreateSubscriberCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ISubscriberRepository> _repository;

        public CreateSubscriberCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<ISubscriberRepository>();

            _unitOfWork.Setup(c => c.GetRepository<ClientSubscriber>(true)).Returns(_repository.Object);
            _unitOfWork.Setup(c => c.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(0).Verifiable();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateSubscriberCommandHandler(null));
        }

        [Fact]
        public async Task Process_Success()
        {
            _repository.Setup(c => c.Insert(It.IsAny<ClientSubscriber>())).Verifiable();

            var handler = new CreateSubscriberCommandHandler(_unitOfWork.Object);

            var id = await handler.Handle(new CreateSubscriberCommand
            {
                Subscriber = new SubscriberModel
                {
                    Name = "name",
                    Permissions = SubscriberPermission.Owner,
                    Phone = "+71234567890",
                    ClientId = 1,
                    IsActive = true
                }
            }, CancellationToken.None);

            _unitOfWork.Verify(c => c.SaveChangesAsync(It.IsAny<bool>()), Times.Once);
            _repository.Verify(c => c.Insert(It.IsAny<ClientSubscriber>()), Times.Once);
        }
    }
}