using Common.Measurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Models;
using Model.Requests;
using ScheduleService.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<VenueDetailsController> _logger;
        private readonly IWeatherService _weatherService;

        public WeatherController(
            ILogger<VenueDetailsController> logger,
            IWeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WeatherData))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeather([Required, FromQuery] GetWeatherRequest request)
        {
            WeatherData weather;
            try
            {
                using var pm = new PerformanceMeter(_logger, MeasurementCategory.WeatherMeasurement);
                weather = await _weatherService.GetCurrentWeatherAsync($"postal_code={request.PostalCode}&country_code={request.CountryCode}");
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(GetWeather)} - {e}");
                throw;
            }

            return Ok(weather);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<IEnumerable<WeatherData>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWeather([Required, FromBody] IEnumerable<GetWeatherByDateAndLocationsRequest> request)
        {
            IEnumerable<IEnumerable<WeatherData>> weather = new List<List<WeatherData>>();
            try
            {
                weather = await _weatherService.GetWeatherAsync(request);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(GetWeather)} - {e}");
                throw;
            }

            return Ok(weather);
        }
    }
}
