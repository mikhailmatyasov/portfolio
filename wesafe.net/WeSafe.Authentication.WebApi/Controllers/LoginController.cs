using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Commands.Login;

namespace WeSafe.Authentication.WebApi.Controllers
{
    /// <summary>
    /// Represents Account operations.
    /// </summary>
    [Route("api/[controller]")]
    [Route("api/account")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoginController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Authorizes user.
        /// </summary>
        /// <param name="model">Sign in model.</param>
        /// <returns>The token.</returns>
        [HttpPost("token")]
        public async Task<IActionResult> TokenAsync([FromBody] LoginCommand model)
        {
            return Ok(await _mediator.Send(model));
        }
    }
}
