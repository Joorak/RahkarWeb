

using System.Security.Claims;

namespace WebApi.Services
{
    public class CurrentUserService : ICurrentUserService
    {

        private readonly IHttpContextAccessor httpContextAccessor;

       public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public int UserId => this.GetCurrentUserId();

        private int GetCurrentUserId()
        {
            var userId = this.httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";
            return int.Parse(userId);
        }
    }
}
