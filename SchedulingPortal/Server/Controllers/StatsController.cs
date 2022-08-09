using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.Stats;
using ScheduleService.Services.Stats;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("stats")]
    public class StatsController : ControllerBase
    {
        private readonly ILogger<StatsController> _logger;
        private readonly IStatsService _statsService;

        public StatsController(ILogger<StatsController> logger, IStatsService statsService)
        {
            _logger = logger;
            _statsService = statsService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<StatDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> GetStatsAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId)
        {
            _logger.LogDebug(nameof(GetStatsAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.StatsMeasurement);
                IEnumerable<StatDto> result = await _statsService.GetStatsAsync(organizationId.ToString(), tournamentId.ToString());
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetStatsAsync), e.ToString());
                throw;
            }
        }
    }
}
