using System;
using BSB.Microservices.NServiceBus.RabbitMQ;
using BSB.Microservices.NServiceBus.Serialization;
using NServiceBus;
using NServiceBus.Pipeline;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// An abstract base class for all BusStartup implementations.
    /// </summary>
    /// <seealso cref="BSB.Microservices.NServiceBus.IBusStartup"/>
    public abstract class BusStartup : IBusStartup
    {
        public BusStartup()
        {
            ThrowExceptions = false;
            ScanAppDomainAssemblies = false;
            ScanAssembliesInNestedDirectories = false;
            UseIncomingPartialTypeResolution = true;
            AssembliesToInclude = (p) => p.All();
            DisableServiceControlForLocalDevelopment = true;
            DisableServiceControl = false;
        }

        /// <summary>
        /// Gets or sets the disable service control flag for local development. This will only be
        /// used if the current environment is local | development.
        /// </summary>
        /// <value>The disable service control for local development.</value>
        public bool DisableServiceControlForLocalDevelopment { get; set; }

        /// <summary>
        /// Gets or sets the pipeline configurator.
        /// </summary>
        /// <value>The pipeline configurator.</value>
        public Action<PipelineSettings> PipelineConfigurator { get; set; }

        /// <summary>
        /// Gets or sets the delegate that controls what assemblies are included in the assembly scan.
        /// </summary>
        /// <value>The message handler assembly predicate builder.</value>
        public Func<AssemblyPredicateBuilder, AssemblyPredicateBuilder> AssembliesToInclude { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [scan assemblies in nested directories]. Default
        /// value is false.
        /// </summary>
        /// <value><c>true</c> if [scan assemblies in nested directories]; otherwise, <c>false</c>.</value>
        public bool ScanAssembliesInNestedDirectories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [scan application domain assemblies]. Default is true.
        /// </summary>
        /// <value><c>true</c> if [scan application domain assemblies]; otherwise, <c>false</c>.</value>
        public bool ScanAppDomainAssemblies { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [throw exceptions] during assembly scanning. The
        /// default is true.
        /// </summary>
        /// <value><c>true</c> if [throw exceptions]; otherwise, <c>false</c>.</value>
        public bool ThrowExceptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [disable service control]. This will disable
        /// service control entirely.
        /// </summary>
        /// <value><c>true</c> if [disable service control]; otherwise, <c>false</c>.</value>
        public bool DisableServiceControl { get; set; }

        /// <summary>
        /// Gets or sets the a delegate that provides assembly names to exclude from scanning.
        /// </summary>
        /// <value>The message handler configuration.</value>
        public Func<AssemblyPredicateBuilder, AssemblyPredicateBuilder> AssembliesToExclude { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use partial type resolution]. This will enable a
        /// message mutator that will strip off any fully qualified assembly info from the inbound
        /// type header.
        /// </summary>
        /// <value><c>true</c> if [use partial type resolution]; otherwise, <c>false</c>.</value>
        public bool UseIncomingPartialTypeResolution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use outgoing partial type renaming]. Setting
        /// this flag will remove any assembly information from the outbound type string.
        /// </summary>
        /// <value><c>true</c> if [use outgoing partial type renaming]; otherwise, <c>false</c>.</value>
        public bool UseOutgoingPartialTypeRenaming { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use outgoing header attribtues]. This will
        /// enable a message mutator that will set an outbound header via a <see cref="Attributes.MessageHeaderAttribute"/>.
        /// </summary>
        /// <value><c>true</c> if [use outgoing header attribtues]; otherwise, <c>false</c>.</value>
        public bool UseOutgoingHeaderAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use subscription attribtues]. This will disable
        /// auto nessage handler registrations and use <see cref="Attributes.SubscriptionAttribute"/>.
        /// </summary>
        /// <value><c>true</c> if [use subscription attribtues]; otherwise, <c>false</c>.</value>
        public bool UseAttributeSubscriptions { get; set; }

        /// <summary>
        /// Gets or sets the limit message processing concurrency to.
        /// </summary>
        /// <value>The limit message processing concurrency to.</value>
        public int? LimitMessageProcessingConcurrencyTo { get; set; }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public abstract string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the name of the ecosystem. This is used for ServiceControl integration.
        /// </summary>
        /// <value>The name of the ecosystem.</value>
        public abstract string EcosystemName { get; set; }

        /// <summary>
        /// Gets or sets the partial type resolver configurator.
        /// </summary>
        /// <value>The partial type resolver configurator.</value>
        public Action<PartialTypeResolver> PartialTypeResolverConfigurator { get; set; }

        /// <summary>
        /// This is used to override/modify low-level endpoint settings.
        /// </summary>
        /// <param name="endpointConfiguration">The endpoint configuration.</param>
        /// <param name="executionEnvironment">The execution environment.</param>
        /// <value>The extended endpoint configurator.</value>
        public virtual void ConfigureEndpoint(EndpointConfiguration endpointConfiguration, ExecutionEnvironment executionEnvironment) { }

        /// <summary>
        /// Invokes the delegate;
        /// </summary>
        /// <returns></returns>
        public AssemblyPredicateBuilder InvokeIncludeDelegate()
        {
            if (AssembliesToInclude != null)
            {
                return AssembliesToInclude(new AssemblyPredicateBuilder());
            }
            return null;
        }

        /// <summary>
        /// Invokes the delegate.
        /// </summary>
        /// <returns></returns>
        public AssemblyPredicateBuilder InvokeExcludeDelegate()
        {
            if (AssembliesToExclude != null)
            {
                return AssembliesToExclude(new AssemblyPredicateBuilder());
            }
            return null;
        }

        public virtual void ConfigureRabbitMq(IConfiguration configuration, ExecutionEnvironment executionEnvironment)
        {
        }
    }

    public class DefaultBusStartup : BusStartup
    {
        public DefaultBusStartup(string serviceName, string ecoSystemName)
        {
            ServiceName = serviceName;
            EcosystemName = ecoSystemName;
            UseIncomingPartialTypeResolution = false;
            UseOutgoingPartialTypeRenaming = false;
            DisableServiceControl = false;
            DisableServiceControlForLocalDevelopment = true;
        }

        public override string ServiceName { get; set; }

        public override string EcosystemName { get; set; }
    }
}