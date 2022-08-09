using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Dto.Common;
using ScheduleService.Services.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("page-header")]
    public class PageHeaderController : ControllerBase
    {
        private readonly ILogger<PageHeaderController> _logger;
        private readonly IPageHeaderService _pageHeaderService;

        public PageHeaderController(ILogger<PageHeaderController> logger, IPageHeaderService pageHeaderService)
        {
            _logger = logger;
            _pageHeaderService = pageHeaderService;
        }

        /// <summary>
        /// Gets data for subpage header by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Subpage header data.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PageHeaderDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetPageHeaderDataAsync(
            [Required, FromQuery] Guid organizationId,
            [Required, FromQuery] Guid tournamentId)
        {
            _logger.LogDebug(nameof(GetPageHeaderDataAsync));
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.PageHeaderMeasurement);
                PageHeaderDto result = await _pageHeaderService.GetPageHeaderDataAsync(organizationId.ToString(), tournamentId.ToString());
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("{ControllerMethod} - {Exception}", nameof(GetPageHeaderDataAsync), e.ToString());
                throw;
            }
        }
    }
}
