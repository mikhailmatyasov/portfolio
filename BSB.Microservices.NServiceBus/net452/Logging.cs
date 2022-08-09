#if NET452
using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NServiceBus.Logging;

namespace BSB.Microservices.NServiceBus
{
    internal interface ILoggingBuilder
    {
        void AddConsole();
    }

    internal static class LoggingExtensions
    {
        public static IServiceCollection AddLogging(this IServiceCollection services, Action<ILoggingBuilder> configure)
        {
            services.AddSingleton(x =>
            {
                var loggerFactory = x.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

                if(loggerFactory == null)
                {
                    return new ConsoleLogger(new ConsoleLoggerProvider((y,z) => z > Microsoft.Extensions.Logging.LogLevel.Debug, true).CreateLogger(nameof(IBus)));
                }

                return loggerFactory.AddConsole().CreateLogger<IBus>();
            });

            return services;
        }
    }

    internal class ConsoleLogger : ILogger<IBus>
    {
        private readonly ILogger _logger;

        public ConsoleLogger(ILogger logger)
        {
            _logger = logger;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }

    internal class MicrosoftLogFactory : LoggingFactoryDefinition
    {
        private Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;

        protected override global::NServiceBus.Logging.ILoggerFactory GetLoggingFactory()
        {
            return new LoggerFactory(_loggerFactory);
        }

        public void UseMsFactory(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            if (_loggerFactory == null)
            {
                _loggerFactory = loggerFactory;
            }
        }
    }

    internal class LoggerFactory : global::NServiceBus.Logging.ILoggerFactory
    {
        Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;

        public LoggerFactory(Microsoft.Extensions.Logging.ILoggerFactory msFactory)
        {
            _loggerFactory = msFactory;
        }

        public ILog GetLogger(Type type)
        {
            return GetLogger(type.Name);
        }

        public ILog GetLogger(string name)
        {
            var logger = _loggerFactory.CreateLogger(name);

            return new Logger(logger);
        }
    }


    internal class Logger : ILog
    {
        ILogger logger;

        public Logger(ILogger logger)
        {
            this.logger = logger;
        }

        public void Debug(string message)
        {
            logger.LogDebug(message);
        }

        public void Debug(string message, Exception exception)
        {
            logger.LogDebug(default(EventId), exception, message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            logger.LogDebug(format, args);
        }

        public void Info(string message)
        {
            logger.LogInformation(message);
        }

        public void Info(string message, Exception exception)
        {
            logger.LogInformation(new EventId(), exception, message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            logger.LogInformation(format, args);
        }

        public void Warn(string message)
        {
            logger.LogWarning(message);
        }

        public void Warn(string message, Exception exception)
        {
            logger.LogWarning(new EventId(), exception, message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            logger.LogWarning(format, args);
        }

        public void Error(string message)
        {
            logger.LogError(message);
        }

        public void Error(string message, Exception exception)
        {
            logger.LogError(new EventId(), exception, message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            logger.LogError(format, args);
        }

        public void Fatal(string message)
        {
            logger.LogCritical(message);
        }

        public void Fatal(string message, Exception exception)
        {
            logger.LogCritical(new EventId(), exception, message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            logger.LogCritical(format, args);
        }

        public bool IsDebugEnabled => logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug);
        public bool IsInfoEnabled => logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information);
        public bool IsWarnEnabled => logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning);
        public bool IsErrorEnabled => logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error);
        public bool IsFatalEnabled => logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical);
    }
}
#endif
