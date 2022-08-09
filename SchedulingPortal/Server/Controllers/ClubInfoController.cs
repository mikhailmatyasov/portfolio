using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.ClubInfo;
using ScheduleService.Services.ClubInfo;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("club-info")]
    public class ClubInfoController : ControllerBase
    {
        private readonly IClubInfoService _clubInfoService;
        private readonly ILogger<ClubInfoController> _logger;

        public ClubInfoController(IClubInfoService clubInfoService, ILogger<ClubInfoController> logger)
        {
            _clubInfoService = clubInfoService;
            _logger = logger;
        }

        /// <summary>
        /// Gets club info by organizationId, tournamentId and clubId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="clubId">Key of the club.</param>
        /// <returns>Club info.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClubInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetClubInfoAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId,
            [Required, FromQuery] Guid clubId)
        {
            _logger.LogDebug(nameof(GetClubInfoAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.ClubInfoMeasurement);
                ClubInfoDto result = await _clubInfoService.GetClubInfoAsync(
                    organizationId.ToString(),
                    tournamentId.ToString(),
                    clubId.ToString());

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetClubInfoAsync), e.ToString());
                throw;
            }
        }
    }
}
