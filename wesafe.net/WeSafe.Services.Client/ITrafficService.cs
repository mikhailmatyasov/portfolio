using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    public interface ITrafficService
    {
        Task<IEnumerable<TrafficCountModel>> GetTraffic(TrafficSearchModel searchModel);

        Task AddTrafficEvents(IEnumerable<TrafficEventModel> trafficEvents);

        Task<IEnumerable<TrafficChartModel>> GetTrafficHourlyChart(TrafficHourlyChartRequest request);
    }
}
