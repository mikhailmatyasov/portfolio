using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Behaviors;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Shared.Enumerations;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.UpdateDeviceType
{
    public class UpdateDeviceTypeValidatorTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IDeviceRepository> _repository;

        public UpdateDeviceTypeValidatorTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IDeviceRepository>();

            _unitOfWork.Setup(c => c.GetRepository<Device>(true)).Returns(_repository.Object);
        }

        [Fact]
        public void Process_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new UpdateDeviceTypeValidator(null));
        }

        [Fact]
        public async Task Process_DeviceNotFound_ThrowsInvalidOperationException()
        {
            _repository.Setup(x => x.FindAsync(It.IsAny<int>())).ReturnsAsync(() => null).Verifiable();

            var validator = new UpdateDeviceTypeValidator(_unitOfWork.Object);

            // assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await validator.Process(new UpdateDeviceTypeCommand
                {
                    DeviceId = 678
                }, CancellationToken.None));

            _repository.Verify(x => x.FindAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task Process_DeviceFound_Success()
        {
            _repository.Setup(x => x.FindAsync(It.IsAny<int>())).ReturnsAsync(() => new Device
            {
                Id = 1,
                DeviceType = DeviceType.Traffic,
                ClientId = null
            }).Verifiable();

            var validator = new UpdateDeviceTypeValidator(_unitOfWork.Object);

            await validator.Process(new UpdateDeviceTypeCommand
            {
                DeviceId = 1,
                DeviceType = DeviceType.PeopleRecognition
            }, CancellationToken.None);

            _repository.Verify(x => x.FindAsync(It.IsAny<int>()), Times.Once);
        }
    }
}