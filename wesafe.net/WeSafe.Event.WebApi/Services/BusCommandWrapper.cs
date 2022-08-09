using MassTransit;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Bus.Contracts.Event;
using WeSafe.Event.WebApi.Services.Abstract;

namespace WeSafe.Event.WebApi.Services
{
    public class BusCommandWrapper : IBusCommandWrapper
    {
        private readonly IBusControl _busControl;

        public BusCommandWrapper(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public Task CreateEvent(ICreateEventContract createEventContract)
        {
            return _busControl.Publish(createEventContract, CancellationToken.None);
        }
    }
}
