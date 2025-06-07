// The configurations for the Core Web API.
using Application.Common.Interfaces;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Utils;
using Scalar.AspNetCore;

//using Scalar.AspNetCore;
using WebApi.Services;
using WebAPI.Filters;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseKestrel();
    //builder.WebHost.UseKestrel(options => options.ConfigureEndpoints());
    //builder.WebHost.UseIISIntegration();
    //builder.WebHost.UseKestrelHttpsConfiguration();
    //builder.WebHost.SuppressStatusMessages(true);
    builder.WebHost.UseUrls(builder.Configuration["HostURL"]!.ToString());

    builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(builder.Configuration));
    

    var allowedOrigins = builder.Configuration["AllowedOrigins"];
    var corsPolicy = "EnableCORS";

    //builder.Services.AddCors(options =>
    //{
    //    options.AddPolicy(corsPolicy, builder =>
    //    {
    //        builder.WithOrigins(allowedOrigins!)
    //            .AllowAnyHeader()
    //            .AllowAnyMethod()
    //            .AllowCredentials();
    //    });
    //});

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("EnableCORS", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    // Inject Architecture Layers
    builder.Services.AddApplicationLayer();
    builder.Services.AddInfrastructureLayer(builder.Configuration);

    builder.Services.AddHttpContextAccessor();
    //builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<ApiExceptionFilterAttribute>();

    builder.Services.AddControllers(options => 
            { 
                options.Filters.Add<ApiExceptionFilterAttribute>(); 
                options.Filters.Add<LogActionFilter>(); 
            });
            //.AddFluentValidation((x => x.AutomaticValidationEnabled = false));
    
    // Add JWT TOKEN Settings
    builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
        .AddJwtBearer(options =>
        {
            options.Audience = builder.Configuration["JwtToken:Audience"];
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromSeconds(1),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(builder.Configuration["JwtToken:SecretKey"]!)),
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidAudience = builder.Configuration["JwtToken:Audience"],
                ValidIssuer = builder.Configuration["JwtToken:Issuer"],
            };
        });

    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

    builder.Services.AddHealthChecks();
    builder.Services.AddAuthorization();
    builder.Services.AddControllers();

    // Stripe Configuration - Secret Key
    StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:SecretKey"];

    builder.Services.AddOpenApi();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var runSeeding = Convert.ToBoolean(builder.Configuration["RunSeedingOnStartup"]);

            if (runSeeding)
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();
                //if (context.Database.IsSqlServer())
                //{
                //    context.Database.Migrate();
                //}

                context.Database.Migrate();

                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<Role>>();

                var rolesSeed = new RolesSeedModel
                {
                    AdminRoleName = builder.Configuration["RolesSeedModel:AdminRoleName"],
                    AdminRoleNormalizedName = builder.Configuration["RolesSeedModel:AdminRoleNormalizedName"],
                    CustomerRoleName = builder.Configuration["RolesSeedModel:CustomerRoleName"],
                    CustomerRoleNormalizedName = builder.Configuration["RolesSeedModel:CustomerRoleNormalizedName"],
                    SupplierRoleName = builder.Configuration["RolesSeedModel:SupplierRoleName"],
                    SupplierRoleNormalizedName = builder.Configuration["RolesSeedModel:SupplierRoleNormalizedName"],
                    UserRoleName = builder.Configuration["RolesSeedModel:UserRoleName"],
                    UserRoleNormalizedName = builder.Configuration["RolesSeedModel:UserRoleNormalizedName"],
                    DefaultRoleName = builder.Configuration["RolesSeedModel:DefaultRoleName"],
                    DefaultRoleNormalizedName = builder.Configuration["RolesSeedModel:DefaultRoleNormalizedName"],
                };
                var adminSeed = new AdminSeedModel
                {
                    FirstName = builder.Configuration["AdminSeedModel:FirstName"],
                    LastName = builder.Configuration["AdminSeedModel:LastName"],
                    Email = builder.Configuration["AdminSeedModel:Email"],
                    Password = builder.Configuration["AdminSeedModel:Password"],
                    RoleName = builder.Configuration["AdminSeedModel:RoleName"],
                };

                await AppDbContextSeed.SeedRolesAsync(roleManager, rolesSeed).ConfigureAwait(false);
                await AppDbContextSeed.SeedAdminUserAsync(userManager, roleManager, adminSeed).ConfigureAwait(false);
                //await ApplicationDbContextSeed.SeedCustomersDataAsync(context).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An error occurred while migrating or seeding the database.");
            throw;
        }
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
        app.MapOpenApi();
        //app.UseSwaggerUi(options =>
        //{
        //    options.DocumentPath = "openapi/v1.json";
        //});
        //app.UseSwaggerUi();
        //app.UseReDoc(options =>
        //{
        //    options.Path = "/redoc";
        //});

        /* use /scalar/v1 at the end of address  */
        app.MapScalarApiReference();
        //app.MapScalarApiReference(options =>
        //{
        //    options.WithTitle("Rahkar API");
        //    options.WithTheme(ScalarTheme.BluePlanet);
        //    options.WithSidebar(false);
        //});
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    // this must be places after 'useRouting' and before 'UseAuthorization'
    app.UseCors(corsPolicy);

    app.UseMiddleware<JwtTokenMiddleware>();

    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    // Security Headers for Website
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        context.Response.Headers.Add("Access-Control-Allow-Origin", allowedOrigins);
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("Referrer-Policy", "same-origin");
        context.Response.Headers.Add("Permissions-Policy", "geolocation=(), camera=()");
#pragma warning disable SA1118 // Parameter should not span multiple lines
        context.Response.Headers.Add(builder.Configuration["ContentPolicy"]!, "default-src "
            + "self  "
            + "https://maxcdn.bootstrapcdn.com  "
            + "https://login.microsoftonline.com "
            + "https://sshmantest.azurewebsites.net "
            + " 'unsafe-inline' 'unsafe-eval'");
#pragma warning restore SA1118 // Parameter should not span multiple lines
        context.Response.Headers.Add("SameSite", "Strict");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        await next();
    });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");

    //await app.RunAsync();
    await app.StartAsync();
    Console.WriteLine($"Application has started at : {string.Join(", ", app.Urls)}");
    await app.WaitForShutdownAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
