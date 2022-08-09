using System;
using BSB.Microservices.NServiceBus.Serialization;
using NServiceBus;
using NServiceBus.Pipeline;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// </summary>
    public interface IBusStartup
    {
        /// <summary>
        /// Gets or sets the assembly scanner configuration. This is used to filter the assemblies
        /// NServiceBus scans for messages.
        /// </summary>
        /// <value>The assembly scanner configuration.</value>
        Func<AssemblyPredicateBuilder, AssemblyPredicateBuilder> AssembliesToInclude { get; set; }

        /// <summary>
        /// Gets or sets the message handler configuration. This is used to filter the assemblies
        /// containing implementations of IHandleMessages.
        /// </summary>
        /// <value>The message handler configuration.</value>
        Func<AssemblyPredicateBuilder, AssemblyPredicateBuilder> AssembliesToExclude { get; set; }


        /// <summary>
        /// Gets or sets the pipeline configurator.
        /// </summary>
        /// <value>
        /// The pipeline configurator.
        /// </value>
        Action<PipelineSettings> PipelineConfigurator { get; set; }
        /// <summary>
        /// Invokes the include delegate.
        /// </summary>
        /// <returns></returns>
        AssemblyPredicateBuilder InvokeIncludeDelegate();

        /// <summary>
        ///Invokes the exclude delegate.
        /// </summary>
        /// <returns></returns>
        AssemblyPredicateBuilder InvokeExcludeDelegate();

        /// <summary>
        /// Gets or sets the partial type resolver configurator.
        /// </summary>
        /// <value>The partial type resolver configurator.</value>
        Action<PartialTypeResolver> PartialTypeResolverConfigurator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [scan assemblies in nested directories]. Default
        /// value is false.
        /// </summary>
        /// <value><c>true</c> if [scan assemblies in nested directories]; otherwise, <c>false</c>.</value>
        bool ScanAssembliesInNestedDirectories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [scan application domain assemblies]. Default is true.
        /// </summary>
        /// <value><c>true</c> if [scan application domain assemblies]; otherwise, <c>false</c>.</value>
        bool ScanAppDomainAssemblies { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [throw exceptions] during assembly scanning. The
        /// default is true.
        /// </summary>
        /// <value><c>true</c> if [throw exceptions]; otherwise, <c>false</c>.</value>
        bool ThrowExceptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use partial type resolution]. This will enable a
        /// message mutator that will strip off any fully qualified assembly info from the inbound
        /// type header.
        /// </summary>
        /// <value><c>true</c> if [use partial type resolution]; otherwise, <c>false</c>.</value>
        bool UseIncomingPartialTypeResolution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use outgoing partial type renaming]. Setting
        /// this flag will remove any assembly information from the outbound type string.
        /// </summary>
        /// <value><c>true</c> if [use outgoing partial type renaming]; otherwise, <c>false</c>.</value>
        bool UseOutgoingPartialTypeRenaming { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use outgoing header attribtues]. This will
        /// enable a message mutator that will set an outbound header via a <see cref="Attributes.MessageHeaderAttribute"/>.
        /// </summary>
        /// <value><c>true</c> if [use outgoing header attribtues]; otherwise, <c>false</c>.</value>
        bool UseOutgoingHeaderAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use subscription attribtues]. This will disable
        /// auto nessage handler registrations and use <see cref="Attributes.SubscriptionAttribute"/>.
        /// </summary>
        /// <value><c>true</c> if [use subscription attribtues]; otherwise, <c>false</c>.</value>
        bool UseAttributeSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the disable service control flag for local development. This will override
        /// any other configuration.
        /// </summary>
        /// <value>The disable service control for local development.</value>
        bool DisableServiceControlForLocalDevelopment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [disable service control]. This will disable
        /// service control entirely.
        /// </summary>
        /// <value><c>true</c> if [disable service control]; otherwise, <c>false</c>.</value>
        bool DisableServiceControl { get; set; }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        string ServiceName { get; }

        /// <summary>
        /// Gets or sets the name of the ecosystem. This is used for ServiceControl integration.
        /// </summary>
        /// <value>The name of the ecosystem.</value>
        string EcosystemName { get; }

        /// <summary>
        /// Gets or sets the limit message processing concurrency to.
        /// </summary>
        /// <value>The limit message processing concurrency to.</value>
        int? LimitMessageProcessingConcurrencyTo { get; set; }

        /// <summary>
        /// This is used to override/modify low-level endpoint settings.
        /// </summary>
        /// <param name="endpointConfiguration">The endpoint configuration.</param>
        /// <param name="executionEnvironment">The execution environment.</param>
        /// <value>The extended endpoint configurator.</value>
        void ConfigureEndpoint(EndpointConfiguration endpointConfiguration, ExecutionEnvironment executionEnvironment);

        /// <summary>
        /// Configures the rabbit mq.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="executionEnvironment">The execution environment.</param>
        void ConfigureRabbitMq(RabbitMQ.IConfiguration configuration, ExecutionEnvironment executionEnvironment);
    }
}