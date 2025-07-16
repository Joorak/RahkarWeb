using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace Infrastructure
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            // Database Configuration
            var _dbProvider = configuration["DbProvider"] ?? "SqlServer";
            var _connectionString = configuration[$"ConnectionStrings:{_dbProvider}"];
            switch (_dbProvider)
            {
                case "Sqlite":
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite(_connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
                    break;
                case "Memory":
                    services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("RahkarDb"));
                    break;
                case "LocalDb":
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(_connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
                    break;
                default:
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(_connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
                    break;
            }

            services.AddDbContext<ReportingContext>(options => options.UseSqlServer(configuration["ConnectionStrings:Default2"]));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            // Inject services
            services.AddTransient<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddTransient<IPersianCalendarService, PersianCalendarService>();
            services.AddTransient<IEmailService, EmailService>();

            // Identity Configuration
            var builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength = 3;
                opt.Password.RequiredUniqueChars = 1;
                opt.User.RequireUniqueEmail = true;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // Custom JWT Authentication (replacing IdentityServer)
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
                    };
                });

            // Authorization Configuration
            //services.AddAuthorizationCore(options =>
            //{
            //    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            //    options.AddPolicy("User", policy => policy.RequireRole("User"));
            //    options.AddPolicy("Customer", policy => policy.RequireRole("Customer"));
            //    options.AddPolicy("Supplier", policy => policy.RequireRole("Supplier"));
            //});
            services.AddAuthorizationCore(config =>
            {
                config.AddPolicy(StringRoleResources.Admin, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Admin));
                config.AddPolicy(StringRoleResources.Customer, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Customer));
                //config.AddPolicy(StringRoleResources.Supplier, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Supplier));
                config.AddPolicy(StringRoleResources.User, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.User));
                config.AddPolicy(StringRoleResources.Default, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Default));
            });
            // Add Test Users only in Development
            if (environment?.IsDevelopment() ?? false)
            {
                services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 3;
                });
                // Note: Test users should be added via seeding or a custom method
            }

            return services;
        }
        public static IServiceCollection AddInfrastructureLayer1(this IServiceCollection services, IConfiguration configuration)
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
                        options.UseSqlServer(_connectionString,b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
                    break;

                default:
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(
                            _connectionString,
                            b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
                    break;

            }

            // Inject services
            services.AddDbContext<ReportingContext>(options => options.UseSqlServer(configuration["ConnectionStrings:Default2"]));
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            services.AddTransient<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddTransient<IPersianCalendarService, PersianCalendarService>();
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
            builder = new IdentityBuilder(builder.UserType, builder.RoleType!, builder.Services);
            

            services.AddAuthorizationCore(config =>
            {
                config.AddPolicy(StringRoleResources.Admin, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Admin));
                //config.AddPolicy(StringRoleResources.Customer, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Customer));
                //config.AddPolicy(StringRoleResources.Supplier, policy => policy.RequireRole(ClaimTypes.Role, StringRoleResources.Supplier));
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
