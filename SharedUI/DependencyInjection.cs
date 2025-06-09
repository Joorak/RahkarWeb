


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedUI
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedUI(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<AppDbContext>(options =>
            //                    options.UseSqlite(configuration["ConnectionStrings:Sqlite"], b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(configuration["BaseAddress"]!.ToString()) });
            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            services.AddScoped<IAccessTokenService, AccessTokenService>();
            services.AddScoped<IHomeService, HomeService>();
            return services;
        }
    }
}
