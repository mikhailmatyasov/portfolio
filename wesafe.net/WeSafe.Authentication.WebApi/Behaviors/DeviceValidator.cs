using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR.Pipeline;
using WeSafe.Authentication.WebApi.Commands.DeviceLogin;
using WeSafe.DAL.Extensions;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Authentication.WebApi.Behaviors
{
    /// <summary>
    /// Represents a device login operation validator.
    /// </summary>
    /// <remarks>
    /// Finds a device by the MAC address and validates its authentication token
    /// with the specified secret from <see cref="DeviceLoginCommand"/>
    /// </remarks>
    public class DeviceValidator : IRequestPreProcessor<DeviceLoginCommand>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructors

        public DeviceValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        #endregion

        public async Task Process(DeviceLoginCommand request, CancellationToken cancellationToken)
        {
            var device = await _unitOfWork.GetDeviceRepository().FindByMacAddressAsync(request.MacAddress, true);

            if ( device == null )
            {
                throw new UnauthorizedException($"The device with MAC address {request.MacAddress} not found");
            }

            if ( device.AuthToken != request.Secret )
            {
                throw new UnauthorizedException($"The device with mac address {request.MacAddress} does not match secret key");
            }
        }
    }
}