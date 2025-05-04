

namespace SharedUI.AuthPolicies
{

    public class AdminRoleHandler : AuthorizationHandler<AdminRoleRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRoleRequirement requirement)
        {
            if (context.User.Identity!.IsAuthenticated)
            {
                var userRole = context.User.Claims.FirstOrDefault(c => c.Type == StringRoleResources.RoleClaim && c.Value == StringRoleResources.Admin);

                if (userRole != null && userRole.Value.Equals(requirement.Role))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
