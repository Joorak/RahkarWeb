// <copyright file="RoleConfiguration.cs" company="Joorak Rezapour">
// Copyright (c) Joorak Rezapour. All rights reserved.
// </copyright>

namespace Infrastructure.Persistence.Configurations.Identity
{
    /// <summary>
    /// The configuration for the entity <see cref="Role"/>.
    /// </summary>
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        /// <summary>
        /// A method to configure an entity.
        /// </summary>
        /// <param name="builder">The builder for configuring the entity metadata.</param>
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("AppRoles");

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(x => x.NormalizedName)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
