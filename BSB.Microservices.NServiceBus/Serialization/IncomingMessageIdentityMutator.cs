using System.Linq;
using System.Threading.Tasks;
using BSB.Microservices.NServiceBus.Common;
using Microsoft.Extensions.Logging;
using NServiceBus.MessageMutator;

namespace BSB.Microservices.NServiceBus.Serialization
{
    /// <summary>
    /// This mutator chops off the fully qualified assembly information from an inboud EnclosedMessageTypes header allowing you to specify
    /// partial types.  NServiceBus will then use it's internal registery of messages (implementations of IMessage) to locate the type.
    /// </summary>
    internal class IncomingMessageIdentityMutator : IMutateIncomingTransportMessages
    {
        private readonly IBusStartup _busStartup;
        private readonly ILogger _logger;

        public IncomingMessageIdentityMutator(IBusStartup busStartup, ILogger logger = null)
        {
            _busStartup = busStartup;
            _logger = logger;
        }

        public Task MutateIncoming(MutateIncomingTransportMessageContext context)
        {
            if (!_busStartup.UseIncomingPartialTypeResolution)
            {
                return Task.FromResult(true);
            }
            if (_busStartup.PartialTypeResolverConfigurator != null)
            {
                var resolver = new PartialTypeResolver();
                _busStartup.PartialTypeResolverConfigurator?.Invoke(resolver);
                resolver.ResolveForMessage(context.Headers);
            }
            else
            {
                var headers = context.Headers;
                var messageTypeKey = "NServiceBus.EnclosedMessageTypes";
                if (!headers.TryGetValue(messageTypeKey, out var messageType))
                {
                    return Task.FromResult(true);
                }
                EnclosedMessageTypes enclosedMessageTypes = messageType;
                if (enclosedMessageTypes.AnySystemTypes())
                {
                    return Task.FromResult(true);
                }
                if (enclosedMessageTypes.AreAnyTypesLoadable())
                {
                    return Task.FromResult(true);
                }

                if (enclosedMessageTypes.MessageTypes.Any())
                {
                    headers[messageTypeKey] = enclosedMessageTypes.SerializeWithPartialTypeNames();
                }
            }
            return Task.FromResult(true);
        }
    }
}