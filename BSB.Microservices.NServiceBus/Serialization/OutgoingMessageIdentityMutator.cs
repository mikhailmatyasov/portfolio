using System.Linq;
using System.Threading.Tasks;
using BSB.Microservices.NServiceBus.Common;
using Microsoft.Extensions.Logging;
using NServiceBus.MessageMutator;

namespace BSB.Microservices.NServiceBus.Serialization
{
    /// <summary>
    /// Mutates outgoing messages.  If enabled, this mutator will truncate any type assembly information .
    /// </summary>
    internal class OutgoingMessageIdentityMutator : IMutateOutgoingTransportMessages
    {
        private readonly IBusStartup _busStartup;
        private readonly ILogger _logger;

        public OutgoingMessageIdentityMutator(IBusStartup busStartup, ILogger logger = null)
        {
            _busStartup = busStartup;
            _logger = logger;
        }

        public Task MutateOutgoing(MutateOutgoingTransportMessageContext context)
        {
            if (!_busStartup.UseOutgoingPartialTypeRenaming)
            {
                return Task.FromResult(true);
            }

            var headers = context.OutgoingHeaders;
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

            if (enclosedMessageTypes.MessageTypes.Any())
            {
                var v = enclosedMessageTypes.SerializeWithPartialTypeNames();
                _logger?.LogDebug($"Transforming EnclosedMessageTypes from:{messageType} to:{v}");
                headers[messageTypeKey] = v;
            }
            return Task.FromResult(true);
        }
    }
}