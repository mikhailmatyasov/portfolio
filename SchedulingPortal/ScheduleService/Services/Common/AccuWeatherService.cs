using Common;
using Common.Cache;
using Common.Measurement;
using Microsoft.Extensions.Logging;
using Model.Dto.Common.Weather;
using Model.Models;
using Model.Requests;
using ScheduleService.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleService.Services.Common
{
    public class AccuWeatherService : IWeatherService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IWeatherProvider _provider;
        private readonly ILogger<AccuWeatherService> _logger;

        public AccuWeatherService(ICacheManager cacheManager, IWeatherProvider provider, ILogger<AccuWeatherService> logger)
        {
            _cacheManager = cacheManager;
            _provider = provider;
            _logger = logger;
        }

        public async Task<WeatherData> GetCurrentWeatherAsync(string parameters)
        {
            string key = $"{parameters}-{DateTime.UtcNow.ToString(Constants.DateTimeHttpPattern)}";
            if (_cacheManager.TryGetData(out WeatherData data, key))
            {
                return data;
            }

            var weather = await GetCurrentWeatherByLocation(parameters);
            _cacheManager.AddData(weather, key);
            return weather;
        }

        public async Task<IEnumerable<IEnumerable<WeatherData>>> GetWeatherAsync(IEnumerable<GetWeatherByDateAndLocationsRequest> request)
        {
            var uniqueLocations = GetUniqueLocations(request);
            var forecasts = await GetForecastsByLocations(uniqueLocations);

            var response = new List<List<WeatherData>>();
            foreach (var dl in request)
            {
                var weatherForecasts = new List<WeatherData>();
                foreach (var location in dl.LocationRequests)
                {
                    string key = $"{location.CountryCode}.{location.PostalCode}";
                    WeatherData weather = GetWeatherByDate(
                            forecasts.WeatherDtos[key],
                            DateTime.ParseExact(dl.Date, Constants.DateTimeHttpPattern, CultureInfo.InvariantCulture));

                    weatherForecasts.Add(weather);
                }

                response.Add(weatherForecasts);
            }

            return response;
        }

        private string GetKey(string countryCode, string postalCode)
        {
            return $"{countryCode}.{postalCode}";
        }

        private UniqueLocations GetUniqueLocations(
            IEnumerable<GetWeatherByDateAndLocationsRequest> request)
        {
            Dictionary<string, GetWeatherByDateRequest> weatherLocations = new Dictionary<string, GetWeatherByDateRequest>();
            foreach (var item in request)
            {
                foreach (var location in item.LocationRequests)
                {
                    string key = GetKey(location.CountryCode, location.PostalCode);

                    if (!weatherLocations.ContainsKey(key))
                    {
                        weatherLocations.Add(key, new GetWeatherByDateRequest
                        {
                            CountryCode = location.CountryCode,
                            PostalCode = location.PostalCode,
                            ForecastLength = location.ForecastLength,
                            Date = item.Date,
                        });
                    }
                }
            }

            return new UniqueLocations
            {
                WeatherLocations = weatherLocations.Select(x => x.Value),
            };
        }

        private bool IsDatesEquals(DateTime firstDate, DateTime secondDate)
        {
            if (firstDate.Year == secondDate.Year &&
                firstDate.Month == secondDate.Month &&
                firstDate.Day == secondDate.Day)
            {
                return true;
            }

            return false;
        }

        private string GetIconUrl(DailyForecastDto forecast, DateTime gameDate)
        {
            string url = "https://www.accuweather.com/images/weathericons";
            if (gameDate.Hour >= 5 && gameDate.Hour < 20)
            {
                // day
                url += $"/{forecast.DayIcon}.svg";
            }
            else
            {
                // night
                url += $"/{forecast.NightIcon}.svg";
            }

            return url;
        }

        private int ConvertToFahrenheits(double temperature)
        {
            return (int)((1.8 * temperature) + 32);
        }

        private int GetFahrenheitDegrees(DailyForecastDto forecast, double temperature)
        {
            if (forecast.TemperatureUnit == "C")
            {
                return ConvertToFahrenheits(temperature);
            }

            return (int)temperature;
        }

        private int GetFahrenheitDegrees(CurrentDataDto forecast)
        {
            if (forecast.TemperatureUnit == "C")
            {
                return ConvertToFahrenheits(forecast.TemperatureValue);
            }

            return forecast.TemperatureValue;
        }

        private WeatherData GetWeatherByDate(FutureWeatherDto dto, DateTime gameDate)
        {
            var gameForecast = dto.Data.DailyForecasts.FirstOrDefault(forecast => IsDatesEquals(DateTime.Parse(forecast.Date), gameDate));
            if (gameForecast == null)
            {
                return null;
            }

            return new WeatherData
            {
                MaxTemperature = GetFahrenheitDegrees(gameForecast, gameForecast.MaximumTemperature),
                MinTemperature = GetFahrenheitDegrees(gameForecast, gameForecast.MinimumTemperature),
                WeatherIconUrl = GetIconUrl(gameForecast, gameDate),
                WeatherServiceLink = gameForecast.Link,
                IsAvailable = true,
            };
        }

        private async Task<Forecasts> GetForecastsByLocations(UniqueLocations locations)
        {
            Forecasts forecasts = new Forecasts();
            foreach (var location in locations.WeatherLocations)
            {
                using var registerWeatherPerformanceMeter = new PerformanceMeter(
                    _logger,
                    MeasurementCategory.WeatherMeasurement,
                    nameof(_provider.RegisterWeatherClick));
                await _provider.RegisterWeatherClick();
                registerWeatherPerformanceMeter.Stop();
                using var futureForecastPerformanceMeter = new PerformanceMeter(
                    _logger,
                    MeasurementCategory.WeatherMeasurement,
                    nameof(_provider.GetFutureForecastByLocation));
                var forecast = await _provider.GetFutureForecastByLocation($"country_code={location.CountryCode}&postal_code={location.PostalCode}");
                futureForecastPerformanceMeter.Stop();

                string key = $"{location.CountryCode}.{location.PostalCode}";
                forecasts.WeatherDtos.Add(key, forecast);
            }

            return forecasts;
        }

        private WeatherData MapCurrentWeatherDto(CurrentWeatherDto dto)
        {
            int fahrenheitDegrees = GetFahrenheitDegrees(dto.Data);
            return new WeatherData
            {
                MaxTemperature = GetFahrenheitDegrees(dto.Data),
                WeatherIconUrl = dto.Data.WeatherIconUrl,
                WeatherServiceLink = !string.IsNullOrEmpty(dto.Data.MobileUrl) ? dto.Data.MobileUrl : dto.Data.DesktopUrl,
                IsAvailable = true,
            };
        }

        private async Task<WeatherData> GetCurrentWeatherByLocation(string locationParameters)
        {
            using var registerWeatherPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.WeatherMeasurement,
                nameof(_provider.RegisterWeatherClick));
            await _provider.RegisterWeatherClick("current");
            registerWeatherPerformanceMeter.Stop();
            using var currentForecastPerformanceMeter = new PerformanceMeter(
                _logger,
                MeasurementCategory.WeatherMeasurement,
                nameof(_provider.GetCurrentForecastByLocation));
            var dto = await _provider.GetCurrentForecastByLocation(locationParameters);
            currentForecastPerformanceMeter.Stop();
            return MapCurrentWeatherDto(dto);
        }

        private class UniqueLocations
        {
            public IEnumerable<GetWeatherByDateRequest> WeatherLocations { get; set; }
        }

        private class Forecasts
        {
            public Dictionary<string, FutureWeatherDto> WeatherDtos { get; set; } = new Dictionary<string, FutureWeatherDto>();
        }
    }
}
