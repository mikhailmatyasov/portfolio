using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Authentication.WebApi.Commands.Users;
using WeSafe.Web.Common.Models;

namespace WeSafe.Authentication.WebApi.Controllers
{
    /// <summary>
    /// Represents Mobile user account operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public UsersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a user. Uses internally by other microservices.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("internal")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserCommand model)
        {
            return Ok(new IdModel<string> { Id = await _mediator.Send(model) });
        }

        #endregion
    }
}