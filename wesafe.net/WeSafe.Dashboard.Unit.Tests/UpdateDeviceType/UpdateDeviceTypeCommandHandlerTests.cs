using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Shared.Enumerations;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.UpdateDeviceType
{
    public class UpdateDeviceTypeCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IDeviceRepository> _repository;

        public UpdateDeviceTypeCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IDeviceRepository>();

            _unitOfWork.Setup(c => c.GetRepository<Device>(true)).Returns(_repository.Object);
            _unitOfWork.Setup(c => c.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(0).Verifiable();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new UpdateDeviceTypeCommandHandler(null));
        }

        [Fact]
        public async Task Process_Success()
        {
            var device = new Device
            {
                Id = 1,
                DeviceType = DeviceType.Alpr,
                ClientId = null
            };

            _repository.Setup(x => x.FindAsync(It.IsAny<int>())).ReturnsAsync(() => device).Verifiable();

            var handler = new UpdateDeviceTypeCommandHandler(_unitOfWork.Object) as IRequestHandler<UpdateDeviceTypeCommand>;

            await handler.Handle(new UpdateDeviceTypeCommand
            {
                DeviceId = 1,
                DeviceType = DeviceType.PeopleRecognition
            }, CancellationToken.None);

            Assert.Equal(DeviceType.PeopleRecognition, device.DeviceType);

            _repository.Verify(x => x.FindAsync(It.IsAny<int>()), Times.Once);
            _unitOfWork.Verify(c => c.SaveChangesAsync(It.IsAny<bool>()), Times.Once);
        }
    }
}