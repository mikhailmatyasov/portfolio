using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.RedCards;
using ScheduleService.Services.RedCards;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("red-cards")]
    public class RedCardsController : ControllerBase
    {
        private readonly IRedCardsService _redCardsService;
        private readonly ILogger<RedCardsController> _logger;

        public RedCardsController(IRedCardsService redCardsService, ILogger<RedCardsController> logger)
        {
            _redCardsService = redCardsService;
            _logger = logger;
        }

        /// <summary>
        /// Gets red cards by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Red cards.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GenderRedCardsDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetRedCardsAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId)
        {
            _logger.LogDebug(nameof(GetRedCardsAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.RedCardsMeasurement);
                var result = await _redCardsService.GetRedCardsAsync(organizationId.ToString(), tournamentId.ToString());
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetRedCardsAsync), e.ToString());
                throw;
            }
        }
    }
}
