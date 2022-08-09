using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Models;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.GetDeviceByToken
{
    public class GetDeviceByTokenCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IDeviceRepository> _repository;
        private readonly Mock<IMapper> _mapper;

        public GetDeviceByTokenCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IDeviceRepository>();

            _unitOfWork.Setup(c => c.GetRepository<Device>(true)).Returns(_repository.Object);
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetDeviceByTokenCommandHandler(null, _mapper.Object));
            Assert.Throws<ArgumentNullException>(() => new GetDeviceByTokenCommandHandler(_unitOfWork.Object, null));
        }

        [Fact]
        public async Task Process_Success()
        {
            var device = new Device
            {
                Id = 1,
                Token = "token123",
                ClientId = null
            };

            _mapper.Setup(c => c.Map<DeviceModel>(It.IsAny<Device>())).Returns<Device>(model => new DeviceModel
            {
                Id = model.Id,
                Token = model.Token
            }).Verifiable();
            _repository.Setup(x => x.FindByTokenAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(() => device).Verifiable();

            var handler = new GetDeviceByTokenCommandHandler(_unitOfWork.Object, _mapper.Object);

            var actual = await handler.Handle(new GetDeviceByTokenCommand
            {
                DeviceToken = "token123",
            }, CancellationToken.None);

            Assert.NotNull(actual);
            Assert.Equal(1, actual.Id);
            Assert.Equal("token123", actual.Token);

            _mapper.Verify(c => c.Map<DeviceModel>(It.IsAny<Device>()), Times.Once);
            _repository.Verify(x => x.FindByTokenAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }
    }
}