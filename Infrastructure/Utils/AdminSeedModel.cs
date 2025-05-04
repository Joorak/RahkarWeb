// <copyright file="AdminSeedModel.cs" company="Joorak Rezapour">
// Copyright (c) Joorak Rezapour. All rights reserved.
// </copyright>

namespace Infrastructure.Utils
{
    /// <summary>
    /// A model of Admin user details.
    /// </summary>
    public class AdminSeedModel
    {
        /// <summary>
        /// Gets or Sets the firstname.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or Sets the lastname.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or Sets the email.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or Sets the password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or Sets the role name.
        /// </summary>
        public string? RoleName { get; set; }
    }
}
