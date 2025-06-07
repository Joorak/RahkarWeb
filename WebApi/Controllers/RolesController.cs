using Infrastructure.Services;
using Serilog;

namespace WebApi.Controllers
{

    [Authorize(Roles = $"{StringRoleResources.Admin}")]
    [ApiController]
    public class RolesController : ControllerBase
    {

        public RolesController(IRoleService roleService)
            : base()
        {
            this.RoleService = roleService;
        }
        private IRoleService RoleService { get; }


        [HttpPost("role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var result = await this.RoleService.CreateRoleAsync(request).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }

        [HttpPut("role")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest request)
        {
            var result = await this.RoleService.UpdateRoleAsync(request).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }

        [HttpDelete("role/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var result = await this.RoleService.DeleteRoleAsync(id).ConfigureAwait(false);
            return result.Successful == true
                ? this.Ok(result)
                : this.BadRequest(result);
        }

        [HttpGet("role/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                var result = await Task.Run(() => RoleService.GetRoleById(id)).ConfigureAwait(false);
                return this.Ok(new RequestResult<RoleResponse> { Item = result, Successful = true, Error = null, Items = null });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new RequestResult<UserResponse> { Item = null, Successful = false, Error = ex.Message, Items = null });
            }
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var result = await Task.Run(() => RoleService.GetRoles()).ConfigureAwait(false);
                return this.Ok(new RequestResult<RoleResponse> { Item = result.FirstOrDefault(), Successful = true, Error = null, Items = null });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new RequestResult<UserResponse> { Item = null, Successful = false, Error = ex.Message, Items = null });
            }
        }

        [HttpGet("rolesAdmin")]
        public async Task<IActionResult> GetRolesForAdmin()
        {
            try
            {
                var result = await Task.Run(() => RoleService.GetRoles()).ConfigureAwait(false);
                return this.Ok(new RequestResult<RoleResponse> { Item = result.FirstOrDefault(), Successful = true, Error = null, Items = null });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new RequestResult<UserResponse> { Item = null, Successful = false, Error = ex.Message, Items = null });
            }
        }
    }
}
