using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Logger.WebApi.Commands.AddLogs;
using WeSafe.Logger.WebApi.Models;

namespace WeSafe.Logger.WebApi.Controllers
{
    /// <summary>
    /// Represents device logs operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class LogsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public LogsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Adds logs to the storage.
        /// </summary>
        /// <param name="logs">Log collection.</param>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddLogs([FromBody] IEnumerable<DeviceLogModelRequest> logs)
        {
            await _mediator.Send(new AddLogsCommand()
            {
                Logs = _mapper.Map<IEnumerable<DeviceLogModel>>(logs)
            });

            return Ok();
        }
    }
}
