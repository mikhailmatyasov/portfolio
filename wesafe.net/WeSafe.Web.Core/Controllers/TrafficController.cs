using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core.Models;
using Direction = WeSafe.Services.Client.Models.Direction;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents Traffic operations.
    /// </summary>
    [ApiController]
    public class TrafficController : ControllerBase
    {
        private ITrafficService _trafficService;

        public TrafficController(ITrafficService trafficService)
        {
            _trafficService = trafficService;
        }

        /// <summary>
        /// Gets counted traffic events.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="request">The filter model.</param>
        /// <returns>The traffic event collection.</returns>
        [HttpGet("api/traffic/{deviceId}")]
        public async Task<IActionResult> GetTrafficCountEvents([FromRoute] int deviceId, [FromQuery]GetTrafficCountEventsRequest request)
        {
            var result = await _trafficService.GetTraffic(new TrafficSearchModel()
            {
                DeviceId = deviceId,
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime
            });

            return Ok(new GetTrafficCountEventsResponse()
            {
                Traffic = result
            });
        }

        [HttpGet("api/traffic/{deviceId}/hourly-chart")]
        public async Task<IActionResult> GetTrafficHourlyChart(int deviceId, int cameraId, string date)
        {
            if ( !DateTime.TryParseExact(date, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal, out var parsedDate) )
            {
                throw new ArgumentException("Invalid date. Use yyyy-MM-dd format exactly.");
            }

            var chart = await _trafficService.GetTrafficHourlyChart(new TrafficHourlyChartRequest
            {
                DeviceId = deviceId,
                CameraId = cameraId,
                Date = parsedDate
            });

            return Ok(chart);
        }

        /// <summary>
        /// Adds traffic events to the storage.
        /// </summary>
        /// <param name="trafficEvents">The traffic ebent collection.</param>
        /// <returns>The action result.</returns>
        [HttpPost("api/traffic")]
        public async Task<IActionResult> TrafficEvents([FromBody] IEnumerable<TrafficEventRequest> trafficEvents)
        {
            if (trafficEvents == null)
                throw new ArgumentNullException(nameof(trafficEvents));

            await _trafficService.AddTrafficEvents(trafficEvents.Select(x => new TrafficEventModel()
            {
                ObjectId = x.ObjectId,
                CameraIp = x.CameraIp,
                DeviceMAC = x.DeviceMAC,
                Direction = x.Direction == Models.Direction.In ? Direction.In : Direction.Out,
                UtcDateTime = x.UtcDateTime
            }));

            return Ok();
        }
    }
}
