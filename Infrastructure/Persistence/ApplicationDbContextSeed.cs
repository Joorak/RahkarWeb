

using Domain.Entities;

namespace Infrastructure.Persistence
{

    public static class ApplicationDbContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<Role> roleManager, RolesSeedModel seedData)
        {
            var adminRole = new Role
            {
                Name = seedData.AdminRoleName,
                NormalizedName = seedData.AdminRoleNormalizedName,
            };
            var customerRole = new Role
            {
                Name = seedData.CustomerRoleName,
                NormalizedName = seedData.CustomerRoleNormalizedName,
            };
            var supplierRole = new Role
            {
                Name = seedData.SupplierRoleName,
                NormalizedName = seedData.SupplierRoleNormalizedName,
            };
            var defaultRole = new Role
            {
                Name = seedData.DefaultRoleName,
                NormalizedName = seedData.DefaultRoleNormalizedName,
            };
            var userRole = new Role
            {
                Name = seedData.UserRoleName,
                NormalizedName = seedData.UserRoleNormalizedName,
            };

            if (roleManager.Roles.All(r => r.Name != adminRole.Name))
            {
                await roleManager.CreateAsync(adminRole).ConfigureAwait(false);
            }
            if (roleManager.Roles.All(r => r.Name != customerRole.Name))
            {
                await roleManager.CreateAsync(customerRole).ConfigureAwait(false);
            }
            if (roleManager.Roles.All(r => r.Name != supplierRole.Name))
            {
                await roleManager.CreateAsync(supplierRole).ConfigureAwait(false);
            }

            if (roleManager.Roles.All(r => r.Name != defaultRole.Name))
            {
                await roleManager.CreateAsync(defaultRole).ConfigureAwait(false);
            }

            if (roleManager.Roles.All(r => r.Name != userRole.Name))
            {
                await roleManager.CreateAsync(userRole).ConfigureAwait(false);
            }
        }

        public static async Task SeedAdminUserAsync(UserManager<User> userManager, RoleManager<Role> roleManager, AdminSeedModel seedData)
        {
            var admin = new User
            {
                UserName = seedData.FirstName + "@" + seedData.LastName,
                Email = seedData.Email,
                FirstName = seedData.FirstName,
                LastName = seedData.LastName,
                IsActive = true,
            };
            var adminRole = roleManager.Roles.Where(x => x.Name == seedData.RoleName).FirstOrDefault();
            if (adminRole == null)
            {
                throw new Exception("The selected role does not exists.");
            }

            if (userManager.Users.All(u => u.Email != admin.Email))
            {
                await userManager.CreateAsync(admin, seedData.Password!).ConfigureAwait(false);
                await userManager.AddToRoleAsync(admin, adminRole.Name!).ConfigureAwait(false);
            }
        }
        //public static async Task SeedCustomersDataAsync(AppDbContext context)
        //{
        //    if (!context.Customers.Any())
        //    {
        //        context.Customers.Add(new Customer
        //        {
        //            FirstName = "Customer Name 1",
        //            LastName = "Customer Family 1",
        //            Email = "CustomerName1@CustomerFamily1.com",
        //            BirthDate = 13500101,
        //            Gender = Domain.Enums.PersonGender.Male
        //        });
        //        context.Customers.Add(new Customer
        //        {
        //            FirstName = "Customer Name 2",
        //            LastName = "Customer Family 2",
        //            Email = "CustomerName2@CustomerFamily2.com",
        //            BirthDate = 13500202,
        //            Gender = Domain.Enums.PersonGender.Female
        //        });
        //        context.Customers.Add(new Customer
        //        {
        //            FirstName = "Customer Name 3",
        //            LastName = "Customer Family 3",
        //            Email = "CustomerName3@CustomerFamily3.com",
        //            BirthDate = 13500303,
        //            Gender = Domain.Enums.PersonGender.Male
        //        });
        //        context.Customers.Add(new Customer
        //        {
        //            FirstName = "Customer Name 4",
        //            LastName = "Customer Family 4",
        //            Email = "CustomerName4@CustomerFamily4.com",
        //            BirthDate = 13500404,
        //            Gender = Domain.Enums.PersonGender.Female
        //        });
        //        context.Customers.Add(new Customer
        //        {
        //            FirstName = "Customer Name 5",
        //            LastName = "Customer Family 5",
        //            Email = "CustomerName5@CustomerFamily5.com",
        //            BirthDate = 13500505,
        //            Gender = Domain.Enums.PersonGender.Male
        //        });

        //        await context.SaveChangesAsync().ConfigureAwait(false);
        //    }
        //}
    }
}
