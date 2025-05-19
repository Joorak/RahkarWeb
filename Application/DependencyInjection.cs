
//using Microsoft.Extensions.DependencyInjection;
//using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
