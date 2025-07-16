
using Application.Models;
using Azure;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Identity.Client;
using Microsoft.Win32;
using System.ComponentModel;
using static Application.Utils.ApiEndpoint;

namespace Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        public AccountService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration)
        {
            this.UserManager = userManager;
            this.RoleManager = roleManager;
            this.Configuration = configuration;
        }

        private UserManager<User> UserManager { get; }

        private RoleManager<Role> RoleManager { get; }

        private IConfiguration Configuration { get; }

        public async Task<RequestResponse> ChangePasswordUserAsync(ChangePasswordRequest changePassword)
        {
            var user = await this.UserManager.FindByIdAsync(changePassword.UserId.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                throw new Exception("The user does not exist");
            }

            if (!await this.UserManager.CheckPasswordAsync(user, changePassword.OldPassword!).ConfigureAwait(false))
            {
                throw new Exception("The credentials are not valid");
            }

            if (!changePassword.NewPassword!.Equals(changePassword.ConfirmNewPassword))
            {
                throw new Exception("Passwords do not match");
            }

            await this.UserManager.ChangePasswordAsync(user, changePassword.OldPassword!, changePassword.NewPassword).ConfigureAwait(false);
            return RequestResponse.Success();
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var result = await this.UserManager.CheckPasswordAsync(user, password).ConfigureAwait(false);
            return result;
        }

        private JwtTokenResponse GenerateToken(JwtTokenRequest jwtTokenRequest)
        {
            var jwtSettings = new JwtTokenConfig
            {
                Secret = Configuration["Jwt:SecretKey"],
                Issuer = Configuration["Jwt:Issuer"],
                Audience = Configuration["Jwt:Audience"],
            };
            var key = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(jwtSettings.Secret!));

            //var userRole = await this.UserManager.GetRolesAsync(user).ConfigureAwait(false);
            var userRole = jwtTokenRequest.Role;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, jwtTokenRequest.AccountId!),
                //new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, userRole),
                //new Claim(StringRoleResources.UserIdClaim, user.Id.ToString()),
            };

            var expiresIn = DateTime.Now.AddSeconds(double.Parse(Configuration["Jwt:AccessTokenExpiration"]!));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = jwtSettings.Audience,
                Issuer = jwtSettings.Issuer,
                Expires = expiresIn,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
            };
            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            return new JwtTokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = (int)(expiresIn - DateTime.Now).TotalSeconds,
            };
        }

        public async Task<RequestResponse<JwtTokenResponse>> LoginAsync(LoginRequest login)
        {
            var user = UserManager.Users.SingleOrDefault(u => u.UserName == login.AccountId);
            if (user == null)
                return RequestResponse<JwtTokenResponse>.Failure("Account not found...");

            if (login.RoleForLogin == StringRoleResources.Customer || login.RoleForLogin == StringRoleResources.Supplier)
            { 
            
            }
            var passwordValid = await CheckPasswordAsync(user, login.PassKey!).ConfigureAwait(false);
            if (passwordValid == false)
            {
                throw new Exception("Email / password incorrect");
            }

            //return await GenerateTokenAsync(new JwtTokenRequest() { AccountId = login.AccountId!, Role = login.RoleForLogin});
            var jwtToken = await Task.Run(() => GenerateToken(new JwtTokenRequest() { AccountId = login.AccountId!, Role = login.RoleForLogin }));
            return new RequestResponse<JwtTokenResponse>
            {
                Successful = true,
                Item = jwtToken
            };
        }

        public async Task<RequestResponse<JwtTokenResponse>> LoginAdminAsync(string accountId)
        {
            var jwtToken = await Task.Run(() => GenerateToken(new JwtTokenRequest() { AccountId = accountId, Role = StringRoleResources.Admin }));
            return new RequestResponse<JwtTokenResponse>
            {
                Successful = true,
                Item = jwtToken
            };
        }

        public async Task<RequestResponse<JwtTokenResponse>> RegisterAsync(RegisterRequest register)
        {
            var existUser = UserManager.Users.SingleOrDefault(u => u.UserName == register.AccountId && u.Roles.Contains(new UserRole() { Role = new Role() { NormalizedName = register.RoleForRegister!.Normalize() } }));
            if (existUser != null)
            {
                //throw new Exception("The user with the unique identifier already exists");
                return RequestResponse<JwtTokenResponse>.Failure("The user with the unique identifier and role already exists");
            }

            var newUser = new User
            {
                UserName = register.AccountId,
                FirstName = register.FirstName,
                LastName = register.LastName,
                IsActive = true,
            };
            if (!register.Password!.Equals(register.ConfirmPassword))
            {
                throw new Exception("Passwords do not match");
            }

            await UserManager.CreateAsync(newUser, register.Password).ConfigureAwait(false);

            var role = await RoleManager.FindByNameAsync(register.RoleForRegister!).ConfigureAwait(false);
            if (role == null)
            {
                throw new Exception("The role does not exist");
            }

            await UserManager.AddToRoleAsync(newUser, role.Name!).ConfigureAwait(false);
            var jwtToken = await Task.Run(() => GenerateToken(new JwtTokenRequest() { AccountId = register.AccountId!, Role = register.RoleForRegister! }));
            return new RequestResponse<JwtTokenResponse>
            {
                Successful = true,
                Item = jwtToken
            };

        }

        public async Task<RequestResponse> ResetPasswordUserAsync(ResetPasswordRequest resetPassword)
        {
            var user = await UserManager.FindByEmailAsync(resetPassword.Email!).ConfigureAwait(false);
            if (user == null)
            {
                throw new Exception("The user does not exist");
            }

            if (!resetPassword.NewPassword!.Equals(resetPassword.NewConfirmPassword))
            {
                throw new Exception("Passwords do not match");
            }

            var token = await UserManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            await UserManager.ResetPasswordAsync(user, token, resetPassword.NewPassword).ConfigureAwait(false);
            return RequestResponse.Success();
        }

        public Task<RequestResponse> ValidateTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendPassKeyAsync(string mobileNumber, string passKey)
        {
            throw new NotImplementedException();
        }
    }
}
