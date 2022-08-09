using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WeSafe.Dashboard.WebApi.Enumerations;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    /// <summary>
    /// Represents a <see cref="VerifyDeviceByTokenCommand"/> handler.
    /// </summary>
    public class VerifyDeviceByTokenCommandHandler : IRequestHandler<VerifyDeviceByTokenCommand, DeviceVerificationStatus>
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public VerifyDeviceByTokenCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        public async Task<DeviceVerificationStatus> Handle(VerifyDeviceByTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var device = await _mediator.Send(new GetDeviceByTokenCommand
                {
                    DeviceToken = request.DeviceToken
                }, cancellationToken);

                if ( device == null )
                {
                    return DeviceVerificationStatus.NotFound;
                }

                if ( device.ClientId != null )
                {
                    return DeviceVerificationStatus.Attached;
                }

                return DeviceVerificationStatus.Exists;
            }
            catch ( Exception e )
            {
                return DeviceVerificationStatus.NotFound;
            }
        }
    }
}