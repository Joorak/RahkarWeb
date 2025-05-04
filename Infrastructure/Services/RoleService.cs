

namespace Infrastructure.Services
{

    public class RoleService : IRoleService
    {
        public RoleService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IMapper mapper)
        {
            this.UserManager = userManager;
            this.RoleManager = roleManager;
            this.Mapper = mapper;
        }


        private UserManager<User> UserManager { get; }


        private RoleManager<Role> RoleManager { get; }


        private IMapper Mapper { get; }


        public async Task<List<string>> CheckUserRolesAsync(User user)
        {
            var roles = await this.UserManager.GetRolesAsync(user).ConfigureAwait(false);
            return roles.ToList();
        }


        public RoleResponse? GetDefaultRole()
        {
            var role = this.RoleManager.Roles
                .TagWith(nameof(this.GetDefaultRole))
                .Where(x => x.Name == StringRoleResources.Default &&
                    x.NormalizedName == StringRoleResources.DefaultNormalized)
                .ProjectTo<RoleResponse>(this.Mapper.ConfigurationProvider)
                .FirstOrDefault();
            return role;
        }


        public RoleResponse? GetUserRole()
        {
            var role = this.RoleManager.Roles
                .TagWith(nameof(this.GetUserRole))
                .Where(x => x.Name == StringRoleResources.User &&
                    x.NormalizedName == StringRoleResources.UserNormalized)
                .ProjectTo<RoleResponse>(this.Mapper.ConfigurationProvider)
                .FirstOrDefault();
            return role;
        }


        public RoleResponse? GetAdminRole()
        {
            var role = this.RoleManager.Roles
                .TagWith(nameof(this.GetAdminRole))
                .Where(x => x.Name == StringRoleResources.Admin &&
                    x.NormalizedName == StringRoleResources.AdminNormalized)
                .ProjectTo<RoleResponse>(this.Mapper.ConfigurationProvider)
                .FirstOrDefault();
            return role;
        }


        public async Task<RequestResponse> SetUserRoleAsync(User user, string role)
        {
            var roles = await this.CheckUserRolesAsync(user).ConfigureAwait(false);
            if (roles.Count == 0)
            {
                await this.UserManager.AddToRoleAsync(user, role).ConfigureAwait(false);
                var roleData = await this.RoleManager.FindByNameAsync(role).ConfigureAwait(false);
                return RequestResponse.Success(roleData!.Id);
            }
            else if (roles.Count > 0)
            {
                await this.UserManager.RemoveFromRoleAsync(user, roles[0]).ConfigureAwait(false);
                await this.UserManager.AddToRoleAsync(user, role).ConfigureAwait(false);
                var roleData = await this.RoleManager.FindByNameAsync(role).ConfigureAwait(false);
                return RequestResponse.Success(roleData!.Id);
            }

            throw new Exception("The user has already a role");
        }


        public List<RoleResponse> GetRoles()
        {
            var result = this.RoleManager.Roles
                .TagWith(nameof(this.GetRoles))
                .Where(x => x.Name != StringRoleResources.Admin &&
                    x.NormalizedName != StringRoleResources.AdminNormalized)
                .ProjectTo<RoleResponse>(this.Mapper.ConfigurationProvider)
                .ToList();
            return result;
        }


        public List<RoleResponse> GetRolesForAdmin()
        {
            var result = this.RoleManager.Roles
                .TagWith(nameof(this.GetRolesForAdmin))
                .ProjectTo<RoleResponse>(this.Mapper.ConfigurationProvider)
                .ToList();
            return result;
        }


        public RoleResponse? GetRoleById(int id)
        {
            var result = this.RoleManager.Roles
                .TagWith(nameof(this.GetRoleById))
                .Where(x => x.Id == id)
                .ProjectTo<RoleResponse>(this.Mapper.ConfigurationProvider)
                .FirstOrDefault();
            return result;
        }


        public RoleResponse? GetRoleByNormalizedName(string normalizedName)
        {
            var result = this.RoleManager.Roles
                .TagWith(nameof(this.GetRoleByNormalizedName))
                .Where(x => x.NormalizedName == normalizedName)
                .ProjectTo<RoleResponse>(this.Mapper.ConfigurationProvider)
                .FirstOrDefault();
            return result;
        }


        public async Task<RequestResponse> CreateRoleAsync(CreateRoleRequest request)
        {
            var role = await this.RoleManager.FindByNameAsync(request.Name!).ConfigureAwait(false);
            if (role != null)
            {
                throw new Exception("The role was already created");
            }

            await this.RoleManager.CreateAsync(new Role
            {
                Name = request.Name,
                NormalizedName = request.Name!.ToUpper(),
            }).ConfigureAwait(false);

            var roleData = await this.RoleManager.FindByNameAsync(request.Name).ConfigureAwait(false);
            return RequestResponse.Success(roleData!.Id);
        }

        public async Task<RequestResponse> UpdateRoleAsync(UpdateRoleRequest request)
        {
            var existsRole = await this.RoleManager.FindByNameAsync(request.Name!).ConfigureAwait(false);
            if (existsRole != null)
            {
                throw new Exception("The new role already exists");
            }

            var role = await this.RoleManager.FindByIdAsync(request.Id.ToString()).ConfigureAwait(false);
            if (role == null)
            {
                throw new Exception("The role was not created");
            }

            role.Name = request.Name;
            role.NormalizedName = request.Name!.ToUpper();

            await this.RoleManager.UpdateAsync(role).ConfigureAwait(false);
            return RequestResponse.Success(role.Id);
        }

        public async Task<RequestResponse> DeleteRoleAsync(int roleId)
        {
            var role = await this.RoleManager.FindByIdAsync(roleId.ToString()).ConfigureAwait(false);
            if (role == null)
            {
                throw new Exception("The role was not found");
            }

            await this.RoleManager.DeleteAsync(role).ConfigureAwait(false);
            return RequestResponse.Success(role.Id);
        }

        public async Task<Role?> FindRoleByIdAsync(int roleId)
        {
            var result = await this.RoleManager.FindByIdAsync(roleId.ToString()).ConfigureAwait(false);
            return result;
        }

        public async Task<Role?> FindRoleByNameAsync(string name)
        {
            var result = await this.RoleManager.FindByNameAsync(name).ConfigureAwait(false);
            return result;
        }
    }
}
