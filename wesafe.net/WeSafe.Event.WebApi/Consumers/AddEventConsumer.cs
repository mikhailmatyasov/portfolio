using MassTransit;
using System;
using System.Threading.Tasks;
using WeSafe.Bus.Contracts.Event;

namespace WeSafe.Event.WebApi.Consumers
{
    public class AddEventConsumer : IConsumer<ICreateEventContract>
    {
        public Task Consume(ConsumeContext<ICreateEventContract> context)
        {
            // [EM]: this implementation will be realized in the next PRs.
            throw new NotImplementedException();
        }
    }
}
