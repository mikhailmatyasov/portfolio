using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.AttachDevice
{
    public class AttachDeviceCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IDeviceRepository> _repository;

        public AttachDeviceCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IDeviceRepository>();

            _unitOfWork.Setup(c => c.GetRepository<Device>(true)).Returns(_repository.Object);
            _unitOfWork.Setup(c => c.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(0).Verifiable();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AttachDeviceCommandHandler(null));
        }

        [Fact]
        public async Task Process_Success()
        {
            var device = new Device
            {
                ClientId = null
            };

            _repository.Setup(x => x.FindByTokenAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(() => device).Verifiable();

            var handler = new AttachDeviceCommandHandler(_unitOfWork.Object) as IRequestHandler<AttachDeviceCommand>;

            await handler.Handle(new AttachDeviceCommand
            {
                DeviceToken = "token",
                ClientId = 1
            }, CancellationToken.None);

            Assert.Equal(1, device.ClientId);

            _repository.Verify(x => x.FindByTokenAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            _unitOfWork.Verify(c => c.SaveChangesAsync(It.IsAny<bool>()), Times.Once);
        }
    }
}