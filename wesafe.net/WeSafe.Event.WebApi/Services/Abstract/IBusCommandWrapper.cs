using System.Threading.Tasks;
using WeSafe.Bus.Contracts.Event;

namespace WeSafe.Event.WebApi.Services.Abstract
{
    public interface IBusCommandWrapper
    {
        Task CreateEvent(ICreateEventContract createEventContract);
    }
}
