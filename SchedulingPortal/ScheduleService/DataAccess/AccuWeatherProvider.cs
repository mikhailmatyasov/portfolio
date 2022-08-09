using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.Common.Weather;
using ScheduleService.DataAccess.TokenProvider;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public class AccuWeatherProvider : IWeatherProvider
    {
        private readonly HttpClient _http;
        private readonly IWeatherTokenProvider _tokenProvider;
        private readonly ILogger<AccuWeatherProvider> _logger;

        public AccuWeatherProvider(HttpClient http, IWeatherTokenProvider tokenProvider, ILogger<AccuWeatherProvider> logger)
        {
            _http = http;
            _tokenProvider = tokenProvider;
            _logger = logger;
            string token = tokenProvider.GetToken();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _http.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        }

        public async Task<CurrentWeatherDto> GetCurrentForecastByLocation(string locationParameters)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(GetCurrentForecastByLocation));
            var response = await _http.GetAsync($"weather/current?{locationParameters}");
            performanceMeter.Stop();
            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException(response.ReasonPhrase ?? "Weather service is not available");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<CurrentWeatherDto>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<FutureWeatherDto> GetFutureForecastByLocation(string locationParameters)
        {
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(GetFutureForecastByLocation));
            var response = await _http.GetAsync($"weather/forecast?{locationParameters}");
            performanceMeter.Stop();
            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException(response.ReasonPhrase ?? "Weather service is not available");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<FutureWeatherDto>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task RegisterWeatherClick(string forecastType)
        {
            var requestBody = new { provider = _tokenProvider.GetProvider() };
            var json = JsonSerializer.Serialize(requestBody);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using var performanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.DataAccessMeasurement,
                nameof(RegisterWeatherClick));
            await _http.PostAsync($"weather/{forecastType}", data);
            performanceMeter.Stop();
        }
    }
}
