using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.WildCards;
using ScheduleService.Services.WildCards;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("wild-cards")]
    public class WildCardsController : ControllerBase
    {
        private readonly IWildCardsService _wildCardsService;
        private readonly ILogger<WildCardsController> _logger;

        public WildCardsController(IWildCardsService wildCardsService, ILogger<WildCardsController> logger)
        {
            _wildCardsService = wildCardsService;
            _logger = logger;
        }

        /// <summary>
        /// Gets wild cards by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Wild cards.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WildCardsDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetWildCardsAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId)
        {
            _logger.LogDebug(nameof(GetWildCardsAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.WildCardsMeasurement);
                WildCardsDto result = await _wildCardsService.GetWildCardsAsync(organizationId.ToString(), tournamentId.ToString());
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetWildCardsAsync), e.ToString());
                throw;
            }
        }
    }
}
