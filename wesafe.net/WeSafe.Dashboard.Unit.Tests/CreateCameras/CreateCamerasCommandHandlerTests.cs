using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using MediatR;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Commands.Camera;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Models;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateCameras
{
    public class CreateCamerasCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ICameraRepository> _cameraRepository;
        private readonly Mock<IDeviceRepository> _deviceRepository;
        private readonly Mock<IMapper> _mapper;

        public CreateCamerasCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _cameraRepository = new Mock<ICameraRepository>();
            _deviceRepository = new Mock<IDeviceRepository>();
            _mapper = new Mock<IMapper>();

            _unitOfWork.Setup(c => c.GetRepository<Camera>(true)).Returns(_cameraRepository.Object);
            _unitOfWork.Setup(c => c.GetRepository<Device>(true)).Returns(_deviceRepository.Object);
            _unitOfWork.Setup(c => c.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(0).Verifiable();

            _mapper.Setup(c => c.Map<Camera>(It.IsAny<CameraBaseModel>())).Returns<CameraBaseModel>(model => new Camera
            {
                CameraName = model.CameraName,
                Login = model.Login,
                Password = model.Password,
                Ip = model.Ip,
                Port = model.Port,
                SpecificRtcpConnectionString = model.SpecificRtcpConnectionString,
            }).Verifiable();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateCamerasCommandHandler(null, _mapper.Object));
            Assert.Throws<ArgumentNullException>(() => new CreateCamerasCommandHandler(_unitOfWork.Object, null));
        }

        [Fact]
        public async Task Process_Success()
        {
            var device = new Device
            {
                Id = 1,
                MACAddress = "00:00:00:00",
                ClientId = null
            };
            var cameras = new CameraBaseModel[]
            {
                new CameraBaseModel()
                {
                    CameraName = "Camera1",
                    Login = "login",
                    Password = "password",
                    Ip = "1.1.1.1",
                    Port = "80",
                    SpecificRtcpConnectionString = "connectionString"
                },
                new CameraBaseModel()
                {
                    CameraName = "Camera2",
                    Login = "login",
                    Password = "password",
                    Ip = "2.2.2.2",
                    Port = "80",
                    SpecificRtcpConnectionString = "connectionString"
                }
            };
            
            _deviceRepository.Setup(x => x.FindByMacAddressAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(() => device).Verifiable();

            var handler = new CreateCamerasCommandHandler(_unitOfWork.Object, _mapper.Object) as IRequestHandler<CreateCamerasCommand>;

            await handler.Handle(new CreateCamerasCommand
            {
                MacAddress = "00:00:00:00",
                Cameras = cameras
            }, CancellationToken.None);

            _mapper.Verify(c => c.Map<Camera>(It.IsAny<CameraBaseModel>()), Times.Exactly(2));
            _unitOfWork.Verify(c => c.SaveChangesAsync(It.IsAny<bool>()), Times.Once);
            _cameraRepository.Verify(c => c.InsertAsync(It.IsAny<IEnumerable<Camera>>(), CancellationToken.None), Times.Once);
        }
    }
}
