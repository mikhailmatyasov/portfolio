using System.Threading.Tasks;
using WeSafe.Bus.Contracts.Event;
using WeSafe.Event.WebApi.Services.Abstract;

namespace WeSafe.Event.Integration.Tests._Stubs
{
    public class StubBusCommandWrapper : IBusCommandWrapper
    {
        public Task CreateEvent(ICreateEventContract createEventContract)
        {
            return Task.CompletedTask;
        }
    }
}
