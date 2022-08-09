using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents Plate events operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PlateEventController : ControllerBase
    {
        private readonly IPlateEventService _plateEventService;

        public PlateEventController(IPlateEventService plateEventService)
        {
            _plateEventService = plateEventService;
        }

        /// <summary>
        /// Adds plate event to the storage.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePlateEventAsync([FromForm] PlateEventModel model)
        {
            if (model == null) return BadRequest();

            await _plateEventService.AddPlateEventAsync(model);

            return Ok();
        }

        /// <summary>
        /// Gets device's plate events.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="searchModel">The filter model.</param>
        /// <returns>The plate event collection.</returns>
        [HttpGet("{deviceId}")]
        public async Task<IActionResult> GetPlateEvents([FromRoute] int deviceId, [FromQuery] PlateEventSearchModel searchModel)
        {
            var result = await _plateEventService.GetPlateEventsAsync(deviceId, searchModel);

            return Ok(result);
        }
    }
}