using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.VenueDetails;
using ScheduleService.Services.VenueDetails;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("venue-details")]
    public class VenueDetailsController : ControllerBase
    {
        private readonly ILogger<VenueDetailsController> _logger;
        private readonly IVenueDetailsService _venueDetailsService;

        public VenueDetailsController(ILogger<VenueDetailsController> logger, IVenueDetailsService venueDetailsService)
        {
            _logger = logger;
            _venueDetailsService = venueDetailsService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VenueDetailsDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> GetVenueDetailsAsync(
            [Required, FromQuery] string organizationId,
            [Required, FromQuery] string venueId)
        {
            _logger.LogDebug(nameof(GetVenueDetailsAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.VenueDetailsMeasurement);
                VenueDetailsDto result = await _venueDetailsService.GetVenueDetailsServiceAsync(organizationId, venueId);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetVenueDetailsAsync), e.ToString());
                throw;
            }
        }
    }
}
