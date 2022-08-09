using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Dashboard.WebApi.Commands.Camera;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Shared.Roles;

namespace WeSafe.Dashboard.WebApi.Controllers
{
    /// <summary>
    /// Represents Camera operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = UserRoles.Administrators)]
    public class CamerasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CamerasController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Gets camera by identifier.
        /// </summary>
        /// <param name="id">The camera identifier.</param>
        /// <returns>The camera.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCameraByIdAsync(int id)
        {
            var camera = await _mediator.Send(new GetCameraByIdCommand(id));

            return Ok(camera);
        }

        /// <summary>
        /// Adds camera collection to the storage.
        /// </summary>
        /// <param name="cameraList">The camera collection model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("{deviceMac}/cameras")]
        public async Task<IActionResult> CreateCamerasAsync([FromRoute] string deviceMac, [FromBody] IEnumerable<CameraBaseModel> cameraList)
        {
            await _mediator.Send(new CreateCamerasCommand()
            {
                MacAddress = deviceMac,
                Cameras = cameraList
            });

            return Ok();
        }
    }
}
