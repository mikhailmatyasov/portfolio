using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using WeSafe.Logger.Abstraction.Services;
using WeSafe.Logger.MongoDbStorage.MapperProfiles;
using WeSafe.Logger.MongoDbStorage.Models;

namespace WeSafe.Logger.MongoDbStorage
{
    public static class Injection
    {
        public static IServiceCollection AddMongoLogStorage(this IServiceCollection services, MongoLogConfiguration configuration)
        {
            services.AddAutoMapper(typeof(LogMapperProfile));
            services.AddSingleton(configuration);
            services.AddScoped<IWeSafeLogStorage, MongoDbLogStorage>();

            return services;
        }
    }
}
