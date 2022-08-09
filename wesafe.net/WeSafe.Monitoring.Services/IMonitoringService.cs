using System.Threading;
using System.Threading.Tasks;

namespace WeSafe.Monitoring.Services
{
    public interface IMonitoringService
    {
        Task ProcessAsync(bool notification, CancellationToken cancellationToken);
    }
}