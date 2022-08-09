using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.VenueDetails;
using ScheduleService.Services.FieldClosures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("field-closures")]
    public class FieldClosuresController : ControllerBase
    {
        private readonly IFieldClosuresService _fieldClosuresService;
        private readonly ILogger<FieldClosuresController> _logger;

        public FieldClosuresController(
            IFieldClosuresService fieldClosuresService,
            ILogger<FieldClosuresController> logger)
        {
            _fieldClosuresService = fieldClosuresService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FieldClosureDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetFieldClosuresAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId)
        {
            _logger.LogDebug(nameof(GetFieldClosuresAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.FieldClosuresMeasurement);
                var result = await _fieldClosuresService.GetFieldClosuresAsync(
                    organizationId.ToString(),
                    tournamentId.ToString());

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetFieldClosuresAsync), e.ToString());
                throw;
            }
        }
    }
}
