using Model.Dto.Common.Weather;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface IWeatherProvider
    {
        Task RegisterWeatherClick(string forecastType = "forecast");

        Task<FutureWeatherDto> GetFutureForecastByLocation(string locationParameters);

        Task<CurrentWeatherDto> GetCurrentForecastByLocation(string locationParameters);
    }
}
