using System;
using System.Collections.Generic;
using NServiceBus.Pipeline;

namespace BSB.Microservices.NServiceBus
{
    public class FluentBusStartup
    {
        internal DefaultBusStartup Startup { get; set; }

        internal ExecutionEnvironment ExecutionEnvironment { get; set; }

        internal FluentRabbitConfiguration RabbitOverrides { get; set; }

        public FluentBusStartup()
        {
            Startup = new DefaultBusStartup(null, null)
            {
                ThrowExceptions = true,
                ScanAppDomainAssemblies = true,
                ScanAssembliesInNestedDirectories = true,
                UseIncomingPartialTypeResolution = false,
                DisableServiceControlForLocalDevelopment = true,
                DisableServiceControl = true,
            };

            RabbitOverrides = new FluentRabbitConfiguration();
        }

        public FluentBusStartup Environment(string environmentName)
        {
            ExecutionEnvironment = new ExecutionEnvironment(environmentName);

            return this;
        }

        public FluentBusStartup Rabbit(Action<FluentRabbitConfiguration> configure)
        {
            configure(RabbitOverrides);

            return this;
        }

        public FluentBusStartup ConfigurePipeline( Action<PipelineSettings> pipelineConfigurator)
        {
            Startup.PipelineConfigurator = pipelineConfigurator;

            return this;
        }
        public FluentBusStartup UseHeaderAttributes(bool useHeaderAttributes = true)
        {
            Startup.UseOutgoingHeaderAttributes = useHeaderAttributes;

            return this;
        }
        public FluentBusStartup LimitMessageProcessingConcurrencyTo(int? max = null)
        {
            Startup.LimitMessageProcessingConcurrencyTo = max;
            return this;
        }

        public FluentBusStartup UseAttributeSubscriptions(bool useAttributeSubscriptions = true)
        {
            Startup.UseAttributeSubscriptions = useAttributeSubscriptions;

            return this;
        }

        public FluentBusStartup Overrides(params Action<DefaultBusStartup>[] overrides)
        {
            foreach (var x in overrides)
            {
                x?.Invoke(Startup);
            }

            return this;
        }

        public FluentBusStartup Validate()
        {
            if (ExecutionEnvironment == null)
            {
                throw new ApplicationException($"{nameof(ExecutionEnvironment)} not configured. Invoke {nameof(Environment)} to configure.");
            }

            return this;
        }

        public static FluentBusStartup Build(Action<FluentBusStartup> configure = null)
        {
            var startup = new FluentBusStartup();

            configure?.Invoke(startup);

            return startup.Validate();
        }
    }

    public class FluentRabbitConfiguration
    {
        internal RabbitMQ.IConfiguration RabbitConfiguration { get; set; } = new RabbitMQ.Configuration();

        public FluentRabbitConfiguration UseConsul(string host = "rabbitmq.service.consul")
        {
            RabbitConfiguration.Host = host;
            RabbitConfiguration.UseConsul = true;

            return this;
        }

        public FluentRabbitConfiguration UseVault()
        {
            RabbitConfiguration.UseVaultCredentials = true;

            return this;
        }

        public FluentRabbitConfiguration UseLocalhostForDevelopment()
        {
            RabbitConfiguration.UseLocalhostForDevelopment = true;

            return this;
        }

        public FluentRabbitConfiguration Overrides(params Action<RabbitMQ.IConfiguration>[] overrides)
        {
            foreach (var x in overrides)
            {
                x?.Invoke(RabbitConfiguration);
            }

            return this;
        }
    }
}