using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Commands.DeviceLogin;

namespace WeSafe.Authentication.WebApi.Controllers
{
    /// <summary>
    /// Represents Device account operations.
    /// </summary>
    [Route("api/[controller]")]
    [Route("api")]
    [ApiController]
    public class DeviceLoginController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public DeviceLoginController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Authenticates device.
        /// </summary>
        /// <param name="model">Sign in model.</param>
        /// <returns>The token.</returns>
        [HttpPost("device/auth")]
        public async Task<IActionResult> TokenAsync([FromBody] DeviceLoginCommand model)
        {
            return Ok(await _mediator.Send(model));
        }

        #endregion
    }
}
