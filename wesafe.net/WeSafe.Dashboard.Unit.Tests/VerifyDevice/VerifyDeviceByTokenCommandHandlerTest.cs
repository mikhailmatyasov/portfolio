using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Enumerations;
using WeSafe.Dashboard.WebApi.Models;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.VerifyDevice
{
    public class VerifyDeviceByTokenCommandHandlerTest
    {
        private readonly Mock<IMediator> _mediator;

        public VerifyDeviceByTokenCommandHandlerTest()
        {
            _mediator = new Mock<IMediator>();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new VerifyDeviceByTokenCommandHandler(null));
        }

        [Fact]
        public async Task Process_CheckExistingDevice_ReturnExists()
        {
            _mediator.Setup(c => c.Send(It.IsAny<GetDeviceByTokenCommand>(), CancellationToken.None))
                     .ReturnsAsync(new DeviceModel());

            var handler = new VerifyDeviceByTokenCommandHandler(_mediator.Object);
            var result = await handler.Handle(new VerifyDeviceByTokenCommand
            {
                DeviceToken = "token123"
            }, CancellationToken.None);

            Assert.Equal(DeviceVerificationStatus.Exists, result);

            _mediator.Verify(c => c.Send(It.IsAny<GetDeviceByTokenCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Process_CheckExistingAndAttachedDevice_ReturnAttached()
        {
            _mediator.Setup(c => c.Send(It.IsAny<GetDeviceByTokenCommand>(), CancellationToken.None))
                     .ReturnsAsync(new DeviceModel
                     {
                         ClientId = 1
                     });

            var handler = new VerifyDeviceByTokenCommandHandler(_mediator.Object);
            var result = await handler.Handle(new VerifyDeviceByTokenCommand
            {
                DeviceToken = "token123"
            }, CancellationToken.None);

            Assert.Equal(DeviceVerificationStatus.Attached, result);

            _mediator.Verify(c => c.Send(It.IsAny<GetDeviceByTokenCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Process_CheckNotExistingDevice_ReturnNotFound()
        {
            _mediator.Setup(c => c.Send(It.IsAny<GetDeviceByTokenCommand>(), CancellationToken.None)).ReturnsAsync(() => null);

            var handler = new VerifyDeviceByTokenCommandHandler(_mediator.Object);
            var result = await handler.Handle(new VerifyDeviceByTokenCommand
            {
                DeviceToken = "token123"
            }, CancellationToken.None);

            Assert.Equal(DeviceVerificationStatus.NotFound, result);

            _mediator.Verify(c => c.Send(It.IsAny<GetDeviceByTokenCommand>(), CancellationToken.None), Times.Once);
        }
    }
}