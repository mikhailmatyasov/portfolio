using Model.Models;
using Model.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.Services.Common
{
    public interface IWeatherService
    {
        Task<WeatherData> GetCurrentWeatherAsync(string parameters);

        Task<IEnumerable<IEnumerable<WeatherData>>> GetWeatherAsync(IEnumerable<GetWeatherByDateAndLocationsRequest> request);
    }
}
