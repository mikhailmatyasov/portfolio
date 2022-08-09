using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.Authentication.WebApi.Behaviors;
using WeSafe.Authentication.WebApi.Commands.DeviceLogin;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Web.Common.Exceptions;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.DeviceLogin
{
    public class DeviceValidatorTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IDeviceRepository> _repository;
        private readonly DeviceLoginCommand _deviceLoginCommand;

        public DeviceValidatorTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IDeviceRepository>();

            _unitOfWork.Setup(c => c.GetRepository<Device>(true)).Returns(_repository.Object);

            _deviceLoginCommand = new DeviceLoginCommand
            {
                MacAddress = "00:30:48:5a:58:65",
                Secret = "1344FS5655"
            };
        }

        [Fact]
        public void Process_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DeviceValidator(null));
        }

        [Fact]
        public async Task Process_DeviceNotFound_ThrowsUnauthorizedException()
        {
            // arrange
            _repository.Setup(x => x.FindByMacAddressAsync(It.IsAny<string>(), true)).ReturnsAsync(() => null);

            var deviceValidator = new DeviceValidator(_unitOfWork.Object);

            // assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () =>
                await deviceValidator.Process(_deviceLoginCommand, CancellationToken.None));
        }

        [Fact]
        public async Task Process_DeviceSecretIsNotEqual_ThrowsUnauthorizedException()
        {
            // arrange
            _repository.Setup(x => x.FindByMacAddressAsync(It.IsAny<string>(), true))
                       .ReturnsAsync(() => new Device
                       {
                           AuthToken = "ERDFSDSDER566"
                       });
            var deviceValidator = new DeviceValidator(_unitOfWork.Object);

            // assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () =>
                await deviceValidator.Process(_deviceLoginCommand, CancellationToken.None));
        }

        [Fact]
        public async Task Process_DeviceFound_Success()
        {
            // arrange
            _repository.Setup(x => x.FindByMacAddressAsync(It.IsAny<string>(), true))
                       .ReturnsAsync(() => new Device
                       {
                           AuthToken = "1344FS5655"
                       });
            var deviceValidator = new DeviceValidator(_unitOfWork.Object);

            await deviceValidator.Process(_deviceLoginCommand, CancellationToken.None);
        }
    }
}