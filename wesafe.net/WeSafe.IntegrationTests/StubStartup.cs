using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeSafe.DAL;
using WeSafe.Services;
using WeSafe.Web;
using WeSafe.Web.Core;

namespace WeSafe.IntegrationTests
{
    public class StubStartup : Startup
    {
        public StubStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddMvc().
                AddApplicationPart(typeof(Startup).Assembly)
                .AddApplicationPart(typeof(BaseStartup).Assembly);
        }

        protected override void AddDbContext(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<WeSafeDbContext>(options => options.UseInMemoryDatabase("1111"));
        }

        protected override void MigrateAndSeed(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WeSafeDbContext>();
                context.Seed(scope);
            }
        }

        protected override void AddHostedServices(IServiceCollection services)
        {
            services.AddScoped<UnhandledExceptionsHandlerHostedService>();
            // This method is empty, because there is no necessity to test hosted services.
            // Moreover, they can crash the app.
        }
    }
}
