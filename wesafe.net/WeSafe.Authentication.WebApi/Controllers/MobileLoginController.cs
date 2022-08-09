using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Authentication.WebApi.Commands.MobileLogin;

namespace WeSafe.Authentication.WebApi.Controllers
{
    /// <summary>
    /// Represents Mobile user account operations.
    /// </summary>
    [Route("api/[controller]")]
    [Route("api")]
    [ApiController]
    public class MobileLoginController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public MobileLoginController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Authenticates mobile user.
        /// </summary>
        /// <param name="model">Sign in model.</param>
        /// <returns>The token.</returns>
        [HttpPost("mobile/signin")]
        public async Task<IActionResult> TokenAsync([FromBody] MobileLoginCommand model)
        {
            return Ok(await _mediator.Send(model));
        }

        #endregion
    }
}