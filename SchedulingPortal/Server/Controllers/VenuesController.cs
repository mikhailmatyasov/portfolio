using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.Venues;
using ScheduleService.Services.Venues;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("venues")]
    public class VenuesController : ControllerBase
    {
        private readonly ILogger<VenuesController> _logger;
        private readonly IVenuesService _venuesService;

        public VenuesController(ILogger<VenuesController> logger, IVenuesService venuesService)
        {
            _logger = logger;
            _venuesService = venuesService;
        }

        /// <summary>
        /// Gets venues data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Tournament games.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VenuesDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetTournamentVenuesAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId)
        {
            _logger.LogDebug(nameof(GetTournamentVenuesAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.VenuesMeasurement);
                VenuesDto result = await _venuesService.GetVenuesAsync(
                    organizationId.ToString(),
                    tournamentId.ToString());
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetTournamentVenuesAsync), e.ToString());
                throw;
            }
        }
    }
}
