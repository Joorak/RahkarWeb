

using Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Services
{

    public class UserService : IUserService
    {
        
        public UserService(
            UserManager<User> userManager,
            IRoleService roleService,
            IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService)
        {
            this.UserManager = userManager;
            this.RoleService = roleService;
            this.UserClaimsPrincipalFactory = userClaimsPrincipalFactory;
            this.AuthorizationService = authorizationService;
        }


        private UserManager<User> UserManager { get; }


        private IRoleService RoleService { get; }


        //private IMapper Mapper { get; }


        private IUserClaimsPrincipalFactory<User> UserClaimsPrincipalFactory { get; }


        private IAuthorizationService AuthorizationService { get; }


        public async Task<RequestResponse> CreateUserAsync(CreateAccountRequest command)
        {
            var existUser = this.UserManager.Users.SingleOrDefault(u => u.UserName == command.AccountId && u.IsActive == true);
            if (existUser != null)
            {
                throw new Exception("The user with the unique identifier already exists");
            }

            var newUser = new User
            {
                UserName = command.FirstName + "@" + command.LastName,
                Email = command.AccountId,
                FirstName = command.FirstName,
                LastName = command.LastName,
                IsActive = true,
            };

            var result = await this.UserManager.CreateAsync(newUser).ConfigureAwait(false);
            if (command.Role!.Length > 0)
            {
                await this.UserManager.AddToRoleAsync(newUser, command.Role).ConfigureAwait(false);
            }

            var user = await this.UserManager.FindByIdAsync(command.AccountId!).ConfigureAwait(false);
            return RequestResponse.Success(user!.Id);
        }


        public async Task<RequestResponse> DeleteUserAsync(int userId)
        {
            var user = this.UserManager.Users.SingleOrDefault(u => u.Id == userId && u.IsActive == true);
            if (user == null)
            {
                throw new Exception("The user doesn't exist");
            }

            user.IsActive = false;

            var result = await this.UserManager.UpdateAsync(user).ConfigureAwait(false);
            return RequestResponse.Success(user.Id);
        }


        public async Task<User?> FindUserByEmailAsync(string email)
        {
            var result = await this.UserManager.FindByEmailAsync(email).ConfigureAwait(false);
            return result;
        }


        public async Task<User?> FindUserByIdAsync(int userId)
        {
            var result = await this.UserManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            return result;
        }


        public UserResponse? GetUserById(int userId)
        {
            var user = this.UserManager.Users
                .TagWith(nameof(this.GetUserById))
                .Where(x => x.Id == userId && x.IsActive == true)
                //.ProjectTo<UserResponse>(this.Mapper.ConfigurationProvider)
                .FirstOrDefault();

            UserResponse userResponse = new() { Id = user!.Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, RoleId = user.Roles.FirstOrDefault()!.RoleId , IsActive = user.IsActive };
            return userResponse;
        }

        public UserResponse? GetUserByEmail(string email)
        {
            var user = this.UserManager.Users
                .TagWith(nameof(this.GetUserByEmail))
                .Where(x => x.Email == email.ToLower() && x.IsActive == true)
                //.ProjectTo<UserResponse>(this.Mapper.ConfigurationProvider)
                .FirstOrDefault();
            UserResponse userResponse = new() { Id = user!.Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, RoleId = user.Roles.FirstOrDefault()!.RoleId, IsActive = user.IsActive };
            return userResponse;
        }

        public async Task<List<string>> GetUserRoleAsync(User user)
        {
            var userRoles = await this.UserManager.GetRolesAsync(user).ConfigureAwait(false);
            return userRoles.ToList();
        }

        public async Task<RequestResponse> UpdateUserAsync(UpdateUserRequest request)
        {
            var existUser = this.UserManager.Users.SingleOrDefault(u => u.Id == request.Id && u.IsActive == true);
            if (existUser == null)
            {
                throw new Exception("The user does not exists");
            }

            existUser.UserName = request.FirstName + "@" + request.LastName;
            existUser.FirstName = request.FirstName;
            existUser.LastName = request.LastName;

            var result = await this.UserManager.UpdateAsync(existUser).ConfigureAwait(false);

            if (request.Role != null)
            {
                var role = await this.RoleService.FindRoleByNameAsync(request.Role).ConfigureAwait(false);
                await this.AssignUserToRoleAsync(new AssignUserToRoleRequest { UserId = existUser.Id, RoleId = role!.Id }).ConfigureAwait(false);
            }

            return RequestResponse.Success(existUser.Id);
        }

        public async Task<RequestResponse> ActivateUserAsync(ActivateUserRequest request)
        {
            var existUser = this.UserManager.Users.SingleOrDefault(u => u.Id == request.Id);
            if (existUser == null)
            {
                throw new Exception("The user does not exists");
            }

            existUser.IsActive = true;

            var result = await this.UserManager.UpdateAsync(existUser).ConfigureAwait(false);
            return RequestResponse.Success(existUser.Id);
        }

        public async Task<RequestResponse> UpdateUserEmailAsync(UpdateUserEmailRequest request)
        {
            var existUser = this.UserManager.Users.SingleOrDefault(u => u.Id == request.UserId &&
                u.Email == request.Email && u.IsActive == true);
            if (existUser == null)
            {
                throw new Exception("The user does not exists");
            }

            var userWithNewEmail = await this.UserManager.FindByEmailAsync(request.NewEmail!).ConfigureAwait(false);
            if (userWithNewEmail != null)
            {
                throw new Exception("The user with the new email value has found in the database");
            }

            existUser.Email = request.NewEmail;

            var result = await this.UserManager.UpdateAsync(existUser).ConfigureAwait(false);
            return RequestResponse.Success(existUser.Id);
        }

        public List<UserResponse> GetUsers()
        {
            var result = this.UserManager.Users
                .TagWith(nameof(this.GetUsers))
                .Where(u => u.IsActive == true)
                //.ProjectTo<UserResponse>(this.Mapper.ConfigurationProvider)
                .ToList();
            var users = new List<UserResponse>();
            foreach (var user in result)
                users.Add(new() { Id = user!.Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, RoleId = user.Roles.FirstOrDefault()!.RoleId, IsActive = user.IsActive });
            return users;
        }

        public List<UserResponse> GetUsersInactive()
        {
            var result = this.UserManager.Users
                .TagWith(nameof(this.GetUsersInactive))
                .Where(u => u.IsActive == false)
                //.ProjectTo<UserResponse>(this.Mapper.ConfigurationProvider)
                .ToList();
            var users = new List<UserResponse>();
            foreach (var user in result)
                users.Add(new() { Id = user!.Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, RoleId = user.Roles.FirstOrDefault()!.RoleId, IsActive = user.IsActive });
            return users;
        }


        public async Task<RequestResponse> AssignUserToRoleAsync(AssignUserToRoleRequest command)
        {
            var user = await this.UserManager.FindByIdAsync(command.UserId.ToString()).ConfigureAwait(false);
            var userRole = await this.UserManager.GetRolesAsync(user!).ConfigureAwait(false);
            await this.UserManager.RemoveFromRoleAsync(user!, userRole[0]).ConfigureAwait(false);

            var role = await this.RoleService.FindRoleByIdAsync(command.RoleId).ConfigureAwait(false);
            await this.UserManager.AddToRoleAsync(user!, role!.Name!).ConfigureAwait(false);

            return RequestResponse.Success(user!.Id);
        }


        public async Task<bool> IsInRoleAsync(int userId, string role)
        {
            var user = this.UserManager.Users.SingleOrDefault(u => u.Id == userId);

            return user != null && await this.UserManager.IsInRoleAsync(user, role).ConfigureAwait(false);
        }


        public async Task<bool> AuthorizeAsync(int userId, string policyName)
        {
            var user = this.UserManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            var principal = await this.UserClaimsPrincipalFactory.CreateAsync(user).ConfigureAwait(false);

            var result = await this.AuthorizationService.AuthorizeAsync(principal, policyName).ConfigureAwait(false);

            return result.Succeeded;
        }
    }
}
