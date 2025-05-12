﻿// <copyright file="RolesSeedModel.cs" company="Joorak Rezapour">
// Copyright (c) Joorak Rezapour. All rights reserved.
// </copyright>

namespace Infrastructure.Utils
{
    /// <summary>
    /// A model of Roles seed.
    /// </summary>
    public class RolesSeedModel
    {

        public string? AdminRoleName { get; set; }

        public string? AdminRoleNormalizedName { get; set; }

        public string? CustomerRoleName { get; set; }
        public string? CustomerRoleNormalizedName { get; set; }
        public string? SupplierRoleName { get; set; }
        public string? SupplierRoleNormalizedName { get; set; }
        public string? DefaultRoleName { get; set; }

        public string? DefaultRoleNormalizedName { get; set; }

        public string? UserRoleName { get; set; }


        public string? UserRoleNormalizedName { get; set; }
    }
}
