﻿// <copyright file="UserLoginConfiguration.cs" company="Joorak Rezapour">
// Copyright (c) Joorak Rezapour. All rights reserved.
// </copyright>

namespace Infrastructure.Persistence.Configurations.Identity
{
    /// <summary>
    /// The configuration for the entity <see cref="UserLogin"/>.
    /// </summary>
    public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        /// <summary>
        /// A method to configure an entity.
        /// </summary>
        /// <param name="builder">The builder for configuring the entity metadata.</param>
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("AppUserLogins");

            builder.HasOne(userLogin => userLogin.User)
                .WithMany(user => user.Logins)
                .HasForeignKey(userLogin => userLogin.UserId);
        }
    }
}
