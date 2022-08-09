using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Authentication.WebApi.Commands.VerifyLogin;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.Authentication.WebApi.Models;

namespace WeSafe.Authentication.WebApi.Controllers
{
    /// <summary>
    /// Represents verify login entry points.
    /// </summary>
    [Route("api/verify-login")]
    [ApiController]
    public class VerifyLoginController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Controllers

        public VerifyLoginController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        /// <summary>
        /// Checks if user name is taken.
        /// </summary>
        /// <param name="model">The user name model.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyLoginAsync([FromBody] VerifyLoginCommand model)
        {
            var status = await _mediator.Send(model);

            // TODO: Fix this result in frontend
            return Ok(new VerifyLoginResult
            {
                Status = status
            });
        }
    }
}