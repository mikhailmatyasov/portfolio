using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.Schedules;
using ScheduleService.Services.Schedules;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("schedules")]
    public class SchedulesController : ControllerBase
    {
        private readonly ILogger<SchedulesController> _logger;
        private readonly ISchedulesService _schedulesService;

        public SchedulesController(ILogger<SchedulesController> logger, ISchedulesService schedulesService)
        {
            _logger = logger;
            _schedulesService = schedulesService;
        }

        /// <summary>
        /// Gets tournament data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Schedules info.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ScheduleDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetSchedulesAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId)
        {
            _logger.LogDebug(nameof(GetSchedulesAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.SchedulesMeasurement);
                ScheduleDto result = await _schedulesService.GetSchedulesAsync(organizationId.ToString(), tournamentId.ToString());
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(GetSchedulesAsync)} - {e}");
                throw;
            }
        }
    }
}
