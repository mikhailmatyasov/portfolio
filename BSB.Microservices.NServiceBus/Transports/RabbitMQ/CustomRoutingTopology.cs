using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using NServiceBus.Transport;
using NServiceBus.Transport.RabbitMQ;
using RabbitMQ.Client;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public interface ICustomRoutingToplogy : IRoutingTopology
    {
        IRoutingTopology Create(bool useDurableExchangesAndQueues, EndpointData endpoint);
    }

    public class CustomRoutingTopology : ICustomRoutingToplogy
    {
        private EndpointData Endpoint { get; set; }
        private bool UseDurableExchangesAndQueues { get; set; }

        private readonly ITopologyRecoveryService _queueService;
        private readonly INamingConventions _namingConventions;
        private readonly ConcurrentDictionary<Type, bool> _configuredTypes;

        public CustomRoutingTopology(ITopologyRecoveryService queueService, INamingConventions namingConventions)
        {
            _queueService = queueService;
            _namingConventions = namingConventions;
            _configuredTypes = new ConcurrentDictionary<Type, bool>();

            Endpoint = Endpoint ?? EndpointData.Default();
        }

        public IRoutingTopology Create(bool useDurableExchangesAndQueues, EndpointData endpoint)
        {
            return new CustomRoutingTopology(_queueService, _namingConventions)
            {
                Endpoint = endpoint,
                UseDurableExchangesAndQueues = useDurableExchangesAndQueues
            };
        }

        public virtual void SetupSubscription(IModel channel, Type type, string subscriberName)
        {
            type = type == typeof(IEvent) ? typeof(object) : type;

            SubscribeType(channel, type);

            channel.ExchangeBind(subscriberName, _namingConventions.GetExchangeName(type), string.Empty);

            _queueService.ExchangeBound(new ExchangeBinding(subscriberName, _namingConventions.GetExchangeName(type), string.Empty));
        }

        public virtual void TeardownSubscription(IModel channel, Type type, string subscriberName)
        {
            TryUnbindExchange(channel, type, subscriberName);
        }

        public virtual void Publish(IModel channel, Type type, OutgoingMessage message, IBasicProperties properties)
        {
            SubscribeType(channel, type);

            channel.BasicPublish(_namingConventions.GetExchangeName(type), GetRoutingKeyHeader(properties), false, properties, message.Body);
        }

        public virtual void Send(IModel channel, string address, OutgoingMessage message, IBasicProperties properties)
        {
            channel.BasicPublish(address, GetRoutingKeyHeader(properties), true, properties, message.Body);
        }

        public virtual void RawSendInCaseOfFailure(IModel channel, string address, byte[] body, IBasicProperties properties)
        {
            channel.BasicPublish(address, GetRoutingKeyHeader(properties), true, properties, body);
        }

        public virtual void Initialize(IModel channel, IEnumerable<string> receivingAddresses, IEnumerable<string> sendingAddresses)
        {
            var queueArgs = new Dictionary<string, object>
            {
                { "x-ha-policy", "all" }
            };

            foreach (var address in receivingAddresses.Concat(sendingAddresses))
            {
                if (IsTopicAddress(address))
                {
                    channel.QueueDeclare(address, UseDurableExchangesAndQueues, false, false, queueArgs);

                    _queueService.QueueDeclared(new Queue(address, UseDurableExchangesAndQueues, false, queueArgs));

                    TryCreateExchange(channel, address, ExchangeType.Topic);

                    channel.QueueBind(address, address, Endpoint.TopicPattern);

                    _queueService.QueueBound(new QueueBinding(address, address, Endpoint.TopicPattern));
                }
                else
                {
                    channel.QueueDeclare(address, UseDurableExchangesAndQueues, false, false, queueArgs);

                    _queueService.QueueDeclared(new Queue(address, UseDurableExchangesAndQueues, false, queueArgs));

                    TryCreateExchange(channel, address, ExchangeType.Fanout);

                    channel.QueueBind(address, address, string.Empty);

                    _queueService.QueueBound(new QueueBinding(address, address, string.Empty));
                }
            }
        }

        public virtual void BindToDelayInfrastructure(IModel channel, string address, string deliveryExchange, string routingKey)
        {
            if (IsTopicAddress(address))
            {
                channel.QueueBind(address, deliveryExchange, routingKey);
            }
            else
            {
                channel.ExchangeBind(address, deliveryExchange, routingKey);

                _queueService.ExchangeBound(new ExchangeBinding(address, deliveryExchange, routingKey));
            }
        }

        private bool IsTopicAddress(string address) => Endpoint.Type == EndpointType.Topic && address == $"{_namingConventions.GetQueueName()}{Endpoint.Suffix}";

        private string GetRoutingKeyHeader(IBasicProperties properties)
        {
            if(properties == null || properties.Headers == null)
            {
                return string.Empty;
            }

            return properties.Headers.TryGetValue("routing-key", out object routingKey)
                ? routingKey?.ToString() ?? string.Empty
                : string.Empty;
        }

        private void SubscribeType(IModel channel, Type type)
        {
            if (type == typeof(object) || _configuredTypes.ContainsKey(type))
            {
                return;
            }

            var typeToProcess = type;
            var baseType = typeToProcess.BaseType;

            TryCreateExchange(channel, _namingConventions.GetExchangeName(typeToProcess));

            while (baseType != null)
            {
                TryCreateExchange(channel, _namingConventions.GetExchangeName(baseType));

                channel.ExchangeBind(_namingConventions.GetExchangeName(baseType), _namingConventions.GetExchangeName(typeToProcess), string.Empty);

                _queueService.ExchangeBound(new ExchangeBinding(_namingConventions.GetExchangeName(baseType), _namingConventions.GetExchangeName(typeToProcess), string.Empty));

                typeToProcess = baseType;
                baseType = typeToProcess.BaseType;
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                var exchangeName = _namingConventions.GetExchangeName(interfaceType);

                TryCreateExchange(channel, exchangeName);

                channel.ExchangeBind(exchangeName, _namingConventions.GetExchangeName(type), string.Empty);

                _queueService.ExchangeBound(new ExchangeBinding(exchangeName, _namingConventions.GetExchangeName(type), string.Empty));
            }

            _configuredTypes[type] = true;
        }

        private void TryCreateExchange(IModel channel, string exchangeName, string exchangeType = ExchangeType.Fanout)
        {
            try
            {
                channel.ExchangeDeclare(exchangeName, exchangeType, UseDurableExchangesAndQueues);
            }
            catch (Exception)
            { }
            finally
            {
                _queueService.ExchangeDeclared(new Exchange(exchangeName, exchangeType, UseDurableExchangesAndQueues));
            }
        }

        private void TryUnbindExchange(IModel channel, Type type, string subscriberName)
        {
            try
            {
                channel.ExchangeUnbind(subscriberName, _namingConventions.GetExchangeName(type), string.Empty, null);
            }
            catch (Exception)
            { }
        }
    }
}
