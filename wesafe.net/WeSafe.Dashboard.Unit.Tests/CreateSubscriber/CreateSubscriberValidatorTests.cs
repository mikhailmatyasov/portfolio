using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Behaviors;
using WeSafe.Dashboard.WebApi.Commands.Subscribers;
using WeSafe.Dashboard.WebApi.Models;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateSubscriber
{
    public class CreateSubscriberValidatorTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ISubscriberRepository> _repository;

        public CreateSubscriberValidatorTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<ISubscriberRepository>();

            _unitOfWork.Setup(c => c.GetRepository<ClientSubscriber>(true)).Returns(_repository.Object);
        }

        [Fact]
        public void Process_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateSubscriberValidator(null));
        }

        [Fact]
        public async Task Process_SubscriberFound_ThrowsInvalidOperationException()
        {
            _repository.Setup(x => x.AnyAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(() => true).Verifiable();

            var validator = new CreateSubscriberValidator(_unitOfWork.Object);

            // assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await validator.Process(new CreateSubscriberCommand
                {
                    Subscriber = new SubscriberModel
                    {
                        Phone = "+71234567890",
                        ClientId = 1
                    }
                }, CancellationToken.None));

            _repository.Verify(x => x.AnyAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Process_ValidParameters_Success()
        {
            _repository.Setup(x => x.AnyAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(() => false).Verifiable();

            var validator = new CreateSubscriberValidator(_unitOfWork.Object);

            await validator.Process(new CreateSubscriberCommand
            {
                Subscriber = new SubscriberModel
                {
                    Phone = "+71234567890",
                    ClientId = 1
                }
            }, CancellationToken.None);

            _repository.Verify(x => x.AnyAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }
    }
}