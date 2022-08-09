using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.Schedules;
using ScheduleService.Services.Standings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("standings")]
    public class StandingsController : ControllerBase
    {
        private readonly IStandingsService _standingsService;
        private readonly ILogger<StandingsController> _logger;

        public StandingsController(IStandingsService standingsService, ILogger<StandingsController> logger)
        {
            _standingsService = standingsService;
            _logger = logger;
        }

        /// <summary>
        /// Gets standings data by organizationId, tournamentId and flightId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Standings.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<StandingsBracketGroupedDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetStandingsAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId)
        {
            _logger.LogDebug(nameof(GetStandingsAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.StandingsMeasurement);
                var result = await _standingsService.GetStandingsAsync(
                organizationId.ToString(),
                tournamentId.ToString());

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetStandingsAsync), e.ToString());
                throw;
            }
        }
    }
}
