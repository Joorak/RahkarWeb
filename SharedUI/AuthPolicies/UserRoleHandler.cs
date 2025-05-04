

namespace SharedUI.AuthPolicies
{

    public class UserRoleHandler : AuthorizationHandler<UserRoleRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRoleRequirement requirement)
        {
            if (context.User.Identity!.IsAuthenticated)
            {
                var userRole = context.User.Claims.FirstOrDefault(c => c.Type == StringRoleResources.RoleClaim && c.Value == StringRoleResources.User);

                if (userRole != null && userRole.Value.Equals(requirement.Role))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
