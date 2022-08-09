using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Dashboard.WebApi.Commands.Camera;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Authentication.Abstract;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Dashboard.WebApi.Controllers
{
    /// <summary>
    /// Represents device operations.
    /// </summary>
    [Route("api/v{version:apiVersion}")]
    [Route("api")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class DevicesController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;
        private readonly ICurrentUser _currentUser;

        #endregion

        #region Constructors

        public DevicesController(IMediator mediator, ICurrentUser currentUser)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets current client devices.
        /// </summary>
        /// <returns>The devices.</returns>
        [HttpGet("client/devices")]
        [Authorize(Roles = UserRoles.Users)]
        public async Task<IActionResult> GetClientDevicesAsync()
        {
            var clientId = TryGetClientId();

            var devices = await _mediator.Send(new GetDevicesByClientIdCommand
            {
                ClientId = clientId
            });

            return Ok(devices);
        }

        /// <summary>
        /// Gets client device by id.
        /// </summary>
        /// <returns>The device.</returns>
        [HttpGet("client/devices/{deviceId}")]
        [Authorize(Roles = UserRoles.Users)]
        public async Task<IActionResult> GetClientDeviceAsync(int deviceId)
        {
            var clientId = TryGetClientId();

            var devices = await _mediator.Send(new GetDeviceByClientIdCommand
            {
                ClientId = clientId,
                DeviceId = deviceId
            });

            return Ok(devices);
        }

        /// <summary>
        /// Binds device to the user.
        /// </summary>
        /// <param name="token">The device token.</param>
        /// <returns>The action result.</returns>
        [HttpPost("client/devices/{token}")]
        [Authorize(Roles = UserRoles.Users)]
        public async Task<IActionResult> BindDeviceToClientAsync([FromRoute] string token)
        {
            var clientId = TryGetClientId();

            await _mediator.Send(new AttachDeviceCommand
            {
                ClientId = clientId,
                DeviceToken = token
            });

            return Ok();
        }

        #endregion

        #region Private methods

        private int TryGetClientId()
        {
            if ( _currentUser.ClientId == null )
            {
                throw new BadRequestException("Client id is required.");
            }

            return _currentUser.ClientId.Value;
        }

        #endregion
    }
}