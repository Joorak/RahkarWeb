

using Application.Models;
using Infrastructure.Services;

namespace WebApi.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {

        public UsersController(IUserService userService)
            : base()
        {
            this.UserService = userService;
        }
        private IUserService UserService { get; }

        [Authorize(Roles = $"{StringRoleResources.Admin}")]
        [HttpPost("user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateAccountRequest request)
        {
            var result = await UserService.CreateUserAsync(request).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }

        [Authorize(Roles = $"{StringRoleResources.Admin}")]
        [HttpPost("userActivate")]
        public async Task<IActionResult> ActivateUser([FromBody] ActivateUserRequest request)
        {
            var result = await UserService.ActivateUserAsync(request).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }

        [Authorize(Roles = $"{StringRoleResources.Admin}, {StringRoleResources.User}, {StringRoleResources.Default}")]
        [HttpPut("user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var result = await UserService.UpdateUserAsync(request).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }

        [Authorize(Roles = $"{StringRoleResources.Admin}")]
        [HttpPut("userEmail")]
        public async Task<IActionResult> UpdateUserEmail([FromBody] UpdateUserEmailRequest request)
        {
            var result = await UserService.UpdateUserEmailAsync(request).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }

        [Authorize(Roles = $"{StringRoleResources.Admin}")]
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await UserService.DeleteUserAsync(id).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }

        [Authorize(Roles = $"{StringRoleResources.Admin}, {StringRoleResources.User}, {StringRoleResources.Default}")]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var result = await Task.Run(() => UserService.GetUserById(id)).ConfigureAwait(false);
                return this.Ok(new RequestResult<UserResponse> { Item = result, Successful = true, Error = null, Items = null });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new RequestResult<UserResponse> { Item = null, Successful = false, Error = ex.Message, Items = null });
            }
        }


        [Authorize(Roles = $"{StringRoleResources.Admin}")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var result = await Task.Run(() => UserService.GetUsers()).ConfigureAwait(false);
                return this.Ok(new RequestResult<UserResponse> { Item = result.FirstOrDefault(), Successful = true, Error = null, Items = result.ToList() });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new RequestResult<UserResponse> { Item = null, Successful = false, Error = ex.Message, Items = null });
            }
        }

        [Authorize(Roles = $"{StringRoleResources.Admin}")]
        [HttpGet("usersInactive")]
        public async Task<IActionResult> GetUsersInactive()
        {
            try
            {
                var result = await Task.Run(() => UserService.GetUsersInactive()).ConfigureAwait(false);
                return this.Ok(new RequestResult<UserResponse> { Item = result.FirstOrDefault(), Successful = true, Error = null, Items = result.ToList() });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new RequestResult<UserResponse> { Item = null, Successful = false, Error = ex.Message, Items = null });
            }
        }
    }
}
