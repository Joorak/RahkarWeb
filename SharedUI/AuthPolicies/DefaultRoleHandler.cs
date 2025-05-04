

namespace SharedUI.AuthPolicies
{

    public class DefaultRoleHandler : AuthorizationHandler<DefaultRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DefaultRoleRequirement requirement)
        {
            if (context.User.Identity!.IsAuthenticated)
            {
                var userRole = context.User.Claims.FirstOrDefault(c => c.Type == StringRoleResources.RoleClaim && c.Value == StringRoleResources.Default);

                if (userRole != null && userRole.Value.Equals(requirement.Role))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
