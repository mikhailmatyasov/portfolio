using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Behaviors;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.AttachDevice
{
    public class AttachDeviceValidatorTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IDeviceRepository> _repository;

        public AttachDeviceValidatorTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IDeviceRepository>();

            _unitOfWork.Setup(c => c.GetRepository<Device>(true)).Returns(_repository.Object);
        }

        [Fact]
        public void Process_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AttachDeviceValidator(null));
        }

        [Fact]
        public async Task Process_DeviceNotFound_ThrowsInvalidOperationException()
        {
            _repository.Setup(x => x.FindByTokenAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(() => null).Verifiable();

            var validator = new AttachDeviceValidator(_unitOfWork.Object);

            // assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await validator.Process(new AttachDeviceCommand
                {
                    DeviceToken = "token",
                    ClientId = 1
                }, CancellationToken.None));

            _repository.Verify(x => x.FindByTokenAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task Process_DeviceAlreadyAttached_ThrowsInvalidOperationException()
        {
            _repository.Setup(x => x.FindByTokenAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(() => new Device
            {
                ClientId = 5
            }).Verifiable();

            var validator = new AttachDeviceValidator(_unitOfWork.Object);

            // assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await validator.Process(new AttachDeviceCommand
                {
                    DeviceToken = "token",
                    ClientId = 1
                }, CancellationToken.None));

            _repository.Verify(x => x.FindByTokenAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task Process_DeviceFound_Success()
        {
            _repository.Setup(x => x.FindByTokenAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(() => new Device
            {
                ClientId = null
            }).Verifiable();

            var validator = new AttachDeviceValidator(_unitOfWork.Object);

            await validator.Process(new AttachDeviceCommand
            {
                DeviceToken = "token",
                ClientId = 1
            }, CancellationToken.None);

            _repository.Verify(x => x.FindByTokenAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }
    }
}