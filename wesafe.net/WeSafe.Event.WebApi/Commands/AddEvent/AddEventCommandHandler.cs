using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Bus.Contracts.Event;
using WeSafe.Event.WebApi.Services.Abstract;

namespace WeSafe.Event.WebApi.Commands.AddEvent
{
    public class AddEventCommandHandler : IRequestHandler<AddEventCommand>
    {
        private readonly IBusCommandWrapper _busCommandWrapper;
        private readonly IMapper _mapper;

        public AddEventCommandHandler(IBusCommandWrapper busCommandWrapper, IMapper mapper)
        {
            _busCommandWrapper = busCommandWrapper;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(AddEventCommand request, CancellationToken cancellationToken)
        {
            var createEventContract = _mapper.Map<ICreateEventContract>(request);
            if (createEventContract == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(AddEventCommand)} not mapped to {nameof(ICreateEventContract)}");
            }

            // [EM]: We need to quickly respond to the server and release it later. All further processing of the event is performed in Consumers.
            await _busCommandWrapper.CreateEvent(createEventContract);

            return Unit.Value;
        }
    }
}
