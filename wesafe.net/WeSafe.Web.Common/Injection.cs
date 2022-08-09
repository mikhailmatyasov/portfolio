using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WeSafe.Web.Common.Authentication;
using WeSafe.Web.Common.Authentication.Abstract;
using WeSafe.Web.Common.Behaviors;
using WeSafe.Web.Common.Exceptions.Handlers;

namespace WeSafe.Web.Common
{
    public static class Injection
    {
        public static IServiceCollection AddCommon(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            serviceCollection.AddScoped<ICurrentUser, WeSafeCurrentUser>();

            return serviceCollection;
        }

        public static IApplicationBuilder UseCommon(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseGlobalExceptionHandler(false);

            return applicationBuilder;
        }
    }
}
