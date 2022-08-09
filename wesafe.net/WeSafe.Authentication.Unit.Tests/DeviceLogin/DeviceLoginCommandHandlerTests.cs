using System;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.Authentication.WebApi.Commands.DeviceLogin;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Authentication;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.DeviceLogin
{
    public class DeviceLoginCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IDeviceRepository> _repository;
        private readonly Mock<IAuthTokenGenerator> _authTokenGenerator;

        public DeviceLoginCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IDeviceRepository>();
            _authTokenGenerator = new Mock<IAuthTokenGenerator>();

            _unitOfWork.Setup(c => c.GetRepository<Device>(It.IsAny<bool>())).Returns(_repository.Object);
            _unitOfWork.Setup(c => c.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(0).Verifiable();
            _authTokenGenerator.Setup(c => c.CreateToken(It.IsAny<TokenRequestModel>()))
                               .Returns<TokenRequestModel>((request) => new TokenResponseModel
                               {
                                   AccessToken = "some_value",
                                   DisplayName = request.Name,
                                   UserName = request.Name,
                                   Role = request.Role,
                                   Demo = request.Demo,
                                   ExpiresAt = request.Expires
                               }).Verifiable();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DeviceLoginCommandHandler(null, null));
            Assert.Throws<ArgumentNullException>(() => new DeviceLoginCommandHandler(_unitOfWork.Object, null));
            Assert.Throws<ArgumentNullException>(() => new DeviceLoginCommandHandler(null, _authTokenGenerator.Object));
        }

        [Fact]
        public async Task Process_Success()
        {
            var device = new Device
            {
                MACAddress = "00:30:48:5a:58:65",
                Name = "Device",
                AuthToken = "init_value"
            };

            // arrange
            _repository.Setup(x => x.FindByMacAddressAsync(It.IsAny<string>(), false))
                       .ReturnsAsync(() => device);

            var handler = new DeviceLoginCommandHandler(_unitOfWork.Object, _authTokenGenerator.Object);

            var token = await handler.Handle(new DeviceLoginCommand
            {
                MacAddress = device.MACAddress,
                Secret = "some_secret"
            }, default);

            Assert.NotNull(token);
            Assert.Equal(device.Name, token.UserName);
            Assert.Equal(device.Name, token.DisplayName);
            Assert.Equal(UserRoles.Devices, token.Role);
            Assert.Equal(token.AccessToken, device.AuthToken);

            _unitOfWork.Verify(c => c.SaveChangesAsync(It.IsAny<bool>()), Times.Once);
            _authTokenGenerator.Verify(c => c.CreateToken(It.IsAny<TokenRequestModel>()), Times.Once);
        }
    }
}