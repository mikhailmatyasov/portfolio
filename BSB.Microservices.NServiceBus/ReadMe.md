# BSB.Microservices.NServiceBus

### Installation

`Install-Package BSB.Microservices.NServiceBus`

### Configuration


```csharp
/**
The most basic means of enabling NServicebus.  The Environment variable should come from
IHostingEnvironment.  local|development|testing|staging|production.  The default is production
*/
public void ConfigureServices(IServiceCollection services){
   services.AddNServiceBus<Transport.RabbitMQ>(Environment);
}
/**
This will actually start your bus.
**/
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseNServiceBus<ISendBus, IReceiveBus>();
         }
```

If you need more control over how NServiceBus is configured, you can supply a startup class deriving from `BusStartup`

```csharp
 internal class NServiceBusStartup : BusStartup
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public NServiceBusStartup(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            //instructs the scanner to include assemblies matching predicate
            AssembliesToInclude = (i) => i.All();
            //instructs the scanner to exclude assemblies matching predicates
            AssembliesToExclude = (e) => e.None();
            ScanAppDomainAssemblies = true;
            ScanAssembliesInNestedDirectories = false;
            ThrowExceptions = true;
            //Disables service control integration for local development
            DisableServiceControlForLocalDevelopment = true;
            _configuration = configuration;
            //Allows you to configure partial type resolution.
            //this is useful if you have messages contained in different assemblies
            UseIncomingPartialTypeResolution = true;
             //strips all assembly info off the outbound messages
            UseOutgoingPartialTypeRenaming = true;
            //if you want to completely disable service control, set to true.
             DisableServiceControl = false;
            //configures a registry of assemblies to use to help resolve a partial type n ame
            PartialTypeResolverConfigurator = (c) => c.RegisterAssemblyNames("Aggregato.Messaging");
        }
        //By default the entry assembly name will be used as your service identifier.  If you want to change that,
        //simply pass a string here
        public override string ServiceName => Constants.AggregatoDataSync;
        //If you are using service control integration, you must supply an ecosystem name.  This is the prefix
        //used to build the monitoring, audit and errors queues for ServiceControl
        public override string EcosystemName => Constants.EcosystemName;

        /**
             Allows you to override RabbitMQ configuration
**/
        public override void ConfigureRabbitMq(IConfiguration configuration, ExecutionEnvironment executionEnvironment)
        {
            if (executionEnvironment.Name.Equals("local", StringComparison.OrdinalIgnoreCase) || executionEnvironment.Name.Equals("development", StringComparison.OrdinalIgnoreCase))
            {
                configuration.Host = _configuration["RabbitMqHost"];
                configuration.Username = _configuration["RabbitMqUsername"];
                configuration.Password = _configuration["RabbitMqPassword"];
            }
            else
            {
                configuration.Host = _configuration["RabbitMqHost"];
            }
        }
    }
...
Now update AddNServiceBus method.

public void ConfigureServices(IServiceCollection services){
   services.AddNServiceBus<Transport.RabbitMQ, NServiceBusStartup>(Environment);
}
```
