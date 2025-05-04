// <copyright file="AccountService.cs" company="Joorak Rezapour">
// Copyright (c) Joorak Rezapour. All rights reserved.
// </copyright>

namespace Infrastructure.Services
{
    /// <summary>
    /// An implementation of <see cref="IAccountService"/>.
    /// </summary>
    public class AccountService : IAccountService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="userManager">The instance of <see cref="UserManager{User}"/> to use.</param>
        /// <param name="roleManager">The instance of <see cref="RoleManager{Role}"/> to use.</param>
        /// <param name="configuration">The instance of <see cref="IConfiguration"/> to use.</param>
        public AccountService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration)
        {
            this.UserManager = userManager;
            this.RoleManager = roleManager;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the instance of <see cref="UserManager{User}"/> to use.
        /// </summary>
        private UserManager<User> UserManager { get; }

        /// <summary>
        /// Gets the instance of <see cref="RoleManager{Role}"/> to use.
        /// </summary>
        private RoleManager<Role> RoleManager { get; }

        /// <summary>
        /// Gets the instance of <see cref="IConfiguration"/> to use.
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var result = await this.UserManager.CheckPasswordAsync(user, password).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc/>
        public async Task<JwtTokenResponse> GenerateToken(User user)
        {
            var jwtSettings = new JwtTokenConfig
            {
                Secret = this.Configuration["JwtToken:SecretKey"],
                Issuer = this.Configuration["JwtToken:Issuer"],
                Audience = this.Configuration["JwtToken:Audience"],
            };
            var key = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(jwtSettings.Secret!));

            var userRole = await this.UserManager.GetRolesAsync(user).ConfigureAwait(false);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, userRole[0]),
                new Claim(StringRoleResources.UserIdClaim, user.Id.ToString()),
            };

            var expiresIn = DateTime.Now.AddDays(1);
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
                Successful = true,
            };
        }

        /// <inheritdoc/>
        public async Task<JwtTokenResponse> LoginAsync(LoginRequest login)
        {
            var user = this.UserManager.Users.SingleOrDefault(u => u.Email == login.Email);
            if (user == null)
            {
                throw new Exception("Email / password incorrect");
            }

            var passwordValid = await this.CheckPasswordAsync(user, login.Password!).ConfigureAwait(false);
            if (passwordValid == false)
            {
                throw new Exception("Email / password incorrect");
            }

            return await this.GenerateToken(user).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<JwtTokenResponse> RegisterAsync(RegisterRequest register)
        {
            var existUser = this.UserManager.Users.SingleOrDefault(u => u.Email == register.Email);
            if (existUser != null)
            {
                throw new Exception("The user with the unique identifier already exists");
            }

            var newUser = new User
            {
                UserName = register.FirstName + "@" + register.LastName,
                Email = register.Email,
                FirstName = register.FirstName,
                LastName = register.LastName,
                IsActive = true,
            };
            if (!register.Password!.Equals(register.ConfirmPassword))
            {
                throw new Exception("Passwords do not match");
            }

            await this.UserManager.CreateAsync(newUser, register.Password).ConfigureAwait(false);

            var role = await this.RoleManager.FindByNameAsync(StringRoleResources.Default).ConfigureAwait(false);
            if (role == null)
            {
                throw new Exception("The role does not exist");
            }

            await this.UserManager.AddToRoleAsync(newUser, role.Name!).ConfigureAwait(false);
            return await this.GenerateToken(newUser).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<RequestResponse> ResetPasswordUserAsync(ResetPasswordRequest resetPassword)
        {
            var user = await this.UserManager.FindByEmailAsync(resetPassword.Email!).ConfigureAwait(false);
            if (user == null)
            {
                throw new Exception("The user does not exist");
            }

            if (!resetPassword.NewPassword!.Equals(resetPassword.NewConfirmPassword))
            {
                throw new Exception("Passwords do not match");
            }

            var token = await this.UserManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            await this.UserManager.ResetPasswordAsync(user, token, resetPassword.NewPassword).ConfigureAwait(false);
            return RequestResponse.Success();
        }
    }
}
