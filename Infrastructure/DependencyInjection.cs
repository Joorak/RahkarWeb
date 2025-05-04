﻿


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration["ConnectionStrings:WebApiConnection"],
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            
            // Inject services
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();

            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IEmailService, EmailService>();

            var builder = services.AddIdentityCore<User>(opt =>
            {
                // configure password options & others
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequiredLength = 6;
                opt.Password.RequiredUniqueChars = 1;
                opt.User.RequireUniqueEmail = true;
            });
            builder = new IdentityBuilder(builder.UserType, builder.RoleType!, builder.Services);
            builder.AddRoles<Role>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddAuthorizationCore(config =>
            {
                config.AddPolicy(StringRoleResources.Admin, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Admin));
                config.AddPolicy(StringRoleResources.User, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.User));
                config.AddPolicy(StringRoleResources.Default, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Default));
            });

            return services;
        }
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionstring)
        {
            services.AddDbContext<AppDbContext>(options =>
                                options.UseSqlite(connectionstring,b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));


            
            return services;
        }
    }
}
