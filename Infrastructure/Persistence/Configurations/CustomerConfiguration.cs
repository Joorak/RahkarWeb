

namespace Infrastructure.Persistence.Configurations
{

    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("AppUsers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(x => x.Email)
                .HasMaxLength(150)
                .IsRequired();
            builder.Property(x => x.BirthDate)
                .IsRequired();

            builder.Property(x => ((byte)x.Gender))
                .IsRequired();
            
        }
    }
}
