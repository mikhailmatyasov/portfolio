using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.Tournament;
using ScheduleService.Services.Tournament;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("tournament")]
    public class TournamentController : ControllerBase
    {
        private readonly ILogger<TournamentController> _logger;
        private readonly ITournamentService _tournamentService;

        public TournamentController(ILogger<TournamentController> logger, ITournamentService tournamentService)
        {
            _logger = logger;
            _tournamentService = tournamentService;
        }

        /// <summary>
        /// Gets tournament data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Tournament info.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TournamentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetTournamentAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId)
        {
            _logger.LogDebug(nameof(GetTournamentAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.TournamentMeasurement);
                var result = await _tournamentService.GetTournamentAsync(organizationId.ToString(), tournamentId.ToString());
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetTournamentAsync), e.ToString());
                throw;
            }
        }
    }
}
