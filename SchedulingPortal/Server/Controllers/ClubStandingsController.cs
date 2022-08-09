using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.ClubStandings;
using ScheduleService.Services.ClubStandings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("club-standings")]
    public class ClubStandingsController : ControllerBase
    {
        private readonly IClubStandingsService _clubStandingsService;
        private readonly ILogger<ClubStandingsController> _logger;

        public ClubStandingsController(IClubStandingsService clubStandingsService, ILogger<ClubStandingsController> logger)
        {
            _clubStandingsService = clubStandingsService;
            _logger = logger;
        }

        /// <summary>
        /// Gets club standings data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Club standings.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ClubStandingsDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetClubStandingsAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId)
        {
            _logger.LogDebug(nameof(GetClubStandingsAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.ClubStandingsMeasurement);
                var result = await _clubStandingsService.GetClubStandingsAsync(
                organizationId.ToString(),
                tournamentId.ToString());

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetClubStandingsAsync), e.ToString());
                throw;
            }
        }
    }
}
