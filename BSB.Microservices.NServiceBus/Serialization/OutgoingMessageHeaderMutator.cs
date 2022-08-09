using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BSB.Microservices.NServiceBus.Attributes;
using NServiceBus.MessageMutator;

namespace BSB.Microservices.NServiceBus.Serialization
{
    public class OutgoingMessageHeaderMutator : IMutateOutgoingTransportMessages
    {
        private static Dictionary<Type, Dictionary<PropertyInfo, MessageHeaderAttribute>> MessageHeaderProperties = new Dictionary<Type, Dictionary<PropertyInfo, MessageHeaderAttribute>>();

        public Task MutateOutgoing(MutateOutgoingTransportMessageContext context)
        {
            var messageType = context.OutgoingMessage.GetType();

            if (!MessageHeaderProperties.TryGetValue(messageType, out Dictionary<PropertyInfo, MessageHeaderAttribute> properties))
            {
                properties = messageType.GetProperties()
                    .Select(x => new
                    {
                        Property = x,
                        HeaderAttribute = x.GetCustomAttribute<MessageHeaderAttribute>()
                    })
                    .Where(x => x.HeaderAttribute != null)
                    .ToDictionary(x => x.Property, x => x.HeaderAttribute);

                MessageHeaderProperties[messageType] = properties;
            }

            foreach (var property in properties.Where(x => !string.IsNullOrEmpty(x.Value.Name)))
            {
                var headerName = property.Value.Name;

                var headerValue = property.Key.GetValue(context.OutgoingMessage)?.ToString();

                if (!string.IsNullOrEmpty(headerValue))
                {
                    context.OutgoingHeaders[headerName] = headerValue;
                }
            }

            return Task.FromResult(0);
        }
    }
}
