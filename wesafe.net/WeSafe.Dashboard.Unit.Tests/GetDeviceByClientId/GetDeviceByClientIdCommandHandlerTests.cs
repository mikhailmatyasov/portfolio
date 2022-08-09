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

namespace WeSafe.Dashboard.Unit.Tests.GetDeviceByClientId
{
    public class GetDeviceByClientIdCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IDeviceRepository> _repository;
        private readonly Mock<IMapper> _mapper;

        public GetDeviceByClientIdCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IDeviceRepository>();

            _unitOfWork.Setup(c => c.GetRepository<Device>(true)).Returns(_repository.Object);
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetDeviceByClientIdCommandHandler(null, _mapper.Object));
            Assert.Throws<ArgumentNullException>(() => new GetDeviceByClientIdCommandHandler(_unitOfWork.Object, null));
        }

        [Fact]
        public async Task Process_Success()
        {
            var device = new Device
            {
                Id = 1,
                Token = "token123",
                ClientId = 1
            };

            _mapper.Setup(c => c.Map<DeviceModel>(It.IsAny<Device>())).Returns<Device>(model => new DeviceModel
            {
                Id = model.Id,
                Token = model.Token,
                ClientId = model.ClientId
            }).Verifiable();
            _repository.Setup(x => x.FindByClientIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(() => device).Verifiable();

            var handler = new GetDeviceByClientIdCommandHandler(_unitOfWork.Object, _mapper.Object);

            var actual = await handler.Handle(new GetDeviceByClientIdCommand
            {
                ClientId = 1,
                DeviceId = 1
            }, CancellationToken.None);

            Assert.NotNull(actual);
            Assert.Equal(1, actual.Id);
            Assert.Equal("token123", actual.Token);
            Assert.Equal(1, actual.ClientId);

            _mapper.Verify(c => c.Map<DeviceModel>(It.IsAny<Device>()), Times.Once);
            _repository.Verify(x => x.FindByClientIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }
    }
}