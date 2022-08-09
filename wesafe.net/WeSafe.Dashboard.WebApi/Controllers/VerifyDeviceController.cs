using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Controllers
{
    /// <summary>
    /// Represents device operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/verify-device")]
    [Route("api/verify-device")]
    [ApiVersion("1.0")]
    [ApiController]
    public class VerifyDeviceController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public VerifyDeviceController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyDeviceAsync([FromBody] VerifyDeviceByTokenCommand model)
        {
            var result = await _mediator.Send(model);

            // TODO: Fix this result in frontend (old is Ok(ExecutionResult.Success()))
            return Ok(new VerifyDeviceResult
            {
                Status = result
            });
        }
    }
}