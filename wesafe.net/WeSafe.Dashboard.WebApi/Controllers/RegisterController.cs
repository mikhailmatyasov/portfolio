using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeSafe.Dashboard.WebApi.Commands.Register;

namespace WeSafe.Dashboard.WebApi.Controllers
{
    /// <summary>
    /// Represents register device operation.
    /// </summary>
    [Route("api/v{version:apiVersion}")]
    [Route("api")]
    [ApiVersion("1.0")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public RegisterController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        [HttpPost("account/signup")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterCommand model)
        {
            // TODO: Change frontend to do authentication process after registration.
            await _mediator.Send(model);

            return Ok();
        }
    }
}