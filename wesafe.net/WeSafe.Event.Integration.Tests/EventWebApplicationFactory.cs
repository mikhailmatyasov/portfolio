using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using WeSafe.DAL;
using WeSafe.Event.Integration.Tests._Stubs;
using WeSafe.Event.WebApi.Services.Abstract;

namespace WeSafe.Event.Integration.Tests
{
    public class EventWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault
                    (d => d.ServiceType == typeof(DbContextOptions<WeSafeDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var busDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(IBusCommandWrapper));
                if (busDescriptor != null)
                {
                    services.Remove(busDescriptor);
                }

                services.AddScoped<IBusCommandWrapper, StubBusCommandWrapper>();
                services.AddDbContext<WeSafeDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting2");
                });
            });
        }
    }
}
