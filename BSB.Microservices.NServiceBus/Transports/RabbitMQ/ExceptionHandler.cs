using RabbitMQ.Client.Exceptions;
using System;
using System.Linq;

namespace BSB.Microservices.NServiceBus.RabbitMQ
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly INamingConventions _namingConventions;

        public ExceptionHandler(INamingConventions namingConventions)
        {
            _namingConventions = namingConventions;
        }

        public static string MissingExchangeError(string exchangeName) => $"Channel has been closed: AMQP close-reason, initiated by Peer, code=404, text=\"NOT_FOUND - no exchange '{exchangeName}' in vhost '/'\", classId=60, methodId=40, cause=";

        public virtual bool ShouldRecoverOnPublishException(Type messageType, Exception exception)
        {
            return IsExchangeMissing(messageType, exception) || IsRabbitMQClientException(exception);
        }

        private bool IsRabbitMQClientException(Exception exception)
        {
            return
                exception.GetType() == typeof(AlreadyClosedException) ||
                exception.GetType() == typeof(ChannelAllocationException) ||
                exception.GetType() == typeof(ConnectFailureException) ||
                exception.GetType() == typeof(BrokerUnreachableException);
        }

        private bool IsExchangeMissing(Type messageType, Exception exception)
        {
            var exchangeName = _namingConventions.GetExchangeName(messageType).ToLower();

            return IsMessage(messageType, exception, MissingExchangeError(exchangeName).ToLower(), "code=404", exchangeName);
        }

        private bool IsMessage(Type messageType, Exception exception, string expectedMessage, params string[] patternMatch)
        {
            var exceptionToCompare = exception;

            while(exceptionToCompare.InnerException != null)
            {
                exceptionToCompare = exception.InnerException;
            }

            var actualMessage = exceptionToCompare.Message?.ToLower();

            if (string.IsNullOrWhiteSpace(actualMessage))
            {
                return false;
            }

            var levenshteinDistance = expectedMessage.GetLevenshteinDistance(actualMessage);

            if(levenshteinDistance == 0)
            {
                return true;
            }

            if(levenshteinDistance < 10)
            {
                return patternMatch.All(x => actualMessage.Contains(x));
            }

            return false;
        }
    }
}
