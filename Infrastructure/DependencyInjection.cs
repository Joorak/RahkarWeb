


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlServer(
            //        configuration["ConnectionStrings:WebApiConnection"],
            //        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
            
            var _dbProvider = configuration["DbProvider"] ?? "SqlServer".ToString();
            var _connectionString = configuration[$"ConnectionStrings:{_dbProvider}"];
            switch (_dbProvider)
            {
                case "Sqlite":
                    services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(_connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
                    break;
                case "LocalDb":
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(
                            _connectionString,
                            b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
                    break;

                default:
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(
                            _connectionString,
                            b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
                    break;

            }

            // Inject services
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();

            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IEmailService, EmailService>();

            var builder = services.AddIdentityCore<User>(opt =>
            {
                // configure password options & others
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength = 3;
                opt.Password.RequiredUniqueChars = 1;
                opt.User.RequireUniqueEmail = true;
            });
            builder.AddRoles<Role>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            builder = new IdentityBuilder(builder.UserType, builder.RoleType, builder.Services);
            

            services.AddAuthorizationCore(config =>
            {
                config.AddPolicy(StringRoleResources.Admin, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Admin));
                config.AddPolicy(StringRoleResources.Customer, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Customer));
                config.AddPolicy(StringRoleResources.Supplier, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Supplier));
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
