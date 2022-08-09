using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Behaviors;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateCameras
{
    public class CreateCamerasValidatorTests
    {
        private const string _macAddress = "00:00:00:00";
        private readonly CreateCamerasValidator _validator;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ICameraRepository> _cameraRepository;
        private readonly Mock<IDeviceRepository> _deviceRepository;

        public CreateCamerasValidatorTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _cameraRepository = new Mock<ICameraRepository>();
            _deviceRepository = new Mock<IDeviceRepository>();
            
            _unitOfWork.Setup(c => c.GetRepository<Camera>(true)).Returns(_cameraRepository.Object);
            _unitOfWork.Setup(c => c.GetRepository<Device>(true)).Returns(_deviceRepository.Object);
            _unitOfWork.Setup(c => c.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(0).Verifiable();

            _validator = new CreateCamerasValidator(_unitOfWork.Object);
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateCamerasValidator(null));
        }

        [Fact]
        public async Task Process_DeviceNotFound_ThrowsInvalidOperationException()
        {
            _deviceRepository.Setup(x => x.FindByMacAddressAsync(It.IsAny<string>(), true)).ReturnsAsync(() => null).Verifiable();

            // assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _validator.Process(new CreateCamerasCommand()
                {
                    MacAddress = _macAddress,
                    Cameras = new List<CameraBaseModel>()
                }, CancellationToken.None));

            _deviceRepository.Verify(x => x.FindByMacAddressAsync(It.IsAny<string>(), true), Times.Once);
        }

        [Fact]
        public async Task Process_DeviceFound_Success()
        {
            _deviceRepository.Setup(x => x.FindByMacAddressAsync(It.IsAny<string>(), true)).ReturnsAsync(() => new Device()).Verifiable();

            // assert
            await _validator.Process(new CreateCamerasCommand()
                {
                    MacAddress = _macAddress,
                    Cameras = new List<CameraBaseModel>()
                }, CancellationToken.None);

            _deviceRepository.Verify(x => x.FindByMacAddressAsync(It.IsAny<string>(), true), Times.Once);
        }

    }
}
