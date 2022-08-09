using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Persistence;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Test.BSB.Microservices.NServiceBus")]
namespace BSB.Microservices.NServiceBus
{
    internal class InProcessContext : IMessageHandlerContext
    {
        public virtual object Response { get; private set; }

        public virtual SynchronizedStorageSession SynchronizedStorageSession => null;

        public string MessageId => null;

        public string ReplyToAddress => null;

        public IReadOnlyDictionary<string, string> MessageHeaders => new Dictionary<string, string>();

        public ContextBag Extensions => new ContextBag();

        public void DoNotContinueDispatchingCurrentMessageToHandlers()
        {
            throw new InvalidOperationException($"Operation not permitted for {nameof(ISendBus.HandleAsync)}.");
        }

        public virtual Task ForwardCurrentMessageTo(string destination)
        {
            throw new InvalidOperationException($"Operation not permitted for {nameof(ISendBus.HandleAsync)}.");
        }

        public virtual Task HandleCurrentMessageLater()
        {
            throw new InvalidOperationException($"Operation not permitted for {nameof(ISendBus.HandleAsync)}.");
        }

        public virtual Task Publish(object message, PublishOptions options)
        {
            throw new InvalidOperationException($"Operation not permitted for {nameof(ISendBus.HandleAsync)}.");
        }

        public virtual Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
        {
            throw new InvalidOperationException($"Operation not permitted for {nameof(ISendBus.HandleAsync)}.");
        }

        public Task Reply(object message, ReplyOptions options)
        {
            Response = message;

            return Task.FromResult(false);
        }

        public Task Reply<T>(Action<T> messageConstructor, ReplyOptions options)
        {
            var typedResponse = Activator.CreateInstance<T>();

            messageConstructor(typedResponse);

            Response = Response;

            return Task.FromResult(false);
        }

        public virtual Task Send(object message, SendOptions options)
        {
            throw new InvalidOperationException($"Operation not permitted for {nameof(ISendBus.HandleAsync)}.");
        }

        public virtual Task Send<T>(Action<T> messageConstructor, SendOptions options)
        {
            throw new InvalidOperationException($"Operation not permitted for {nameof(ISendBus.HandleAsync)}.");
        }
    }
}
