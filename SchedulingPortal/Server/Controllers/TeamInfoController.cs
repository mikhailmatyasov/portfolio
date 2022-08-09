using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.TeamInfo;
using ScheduleService.Services.TeamInfo;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("team-info")]
    public class TeamInfoController : ControllerBase
    {
        private readonly ITeamInfoService _teamInfoService;
        private readonly ILogger<TeamInfoController> _logger;

        public TeamInfoController(ITeamInfoService teamInfoService, ILogger<TeamInfoController> logger)
        {
            _teamInfoService = teamInfoService;
            _logger = logger;
        }

        /// <summary>
        /// Gets team info by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="teamId">Key of the team.</param>
        /// <returns>Team info.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeamInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetTeamInfoAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId,
            [Required, FromQuery] Guid teamId)
        {
            _logger.LogDebug(nameof(GetTeamInfoAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.TeamInfoMeasurement);
                TeamInfoDto result = await _teamInfoService.GetTeamInfoAsync(organizationId.ToString(), tournamentId.ToString(), teamId.ToString());
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetTeamInfoAsync), e.ToString());
                throw;
            }
        }
    }
}
