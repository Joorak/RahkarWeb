

namespace Infrastructure.Persistence
{

    public class AppDbContext : IdentityDbContext<User, Role, int,
        UserClaim, UserRole, UserLogin,
        RoleClaim, UserToken>, IApplicationDbContext
    {

        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected AppDbContext(
            DbContextOptions options)

            : base(options)
        {
        }

        protected AppDbContext()
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<CarouselItem> CarouselItems { get; set; }
        public DbSet<SellableItem> SellableItems { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.ApplyConfiguration(new RoleClaimConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            builder.ApplyConfiguration(new UserLoginConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
            builder.ApplyConfiguration(new UserTokenConfiguration());

            //builder.ApplyConfiguration(new CustomerConfiguration());
        }

        /// <summary>
        /// Configure db context.
        /// </summary>
        /// <param name="optionsBuilder">The model builder of the db context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/Infrastructure/Persistence/Interceptors/AuditableEntitySaveChangesInterceptor.cs
            // optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
        }
    }
}
