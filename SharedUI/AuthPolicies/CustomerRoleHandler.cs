

namespace SharedUI.AuthPolicies
{

    public class CustomerRoleHandler : AuthorizationHandler<CustomerRoleRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomerRoleRequirement requirement)
        {
            if (context.User.Identity!.IsAuthenticated)
            {
                var defaultRole = context.User.Claims.FirstOrDefault(c => c.Type == StringRoleResources.RoleClaim && c.Value == StringRoleResources.Default);
                var userRole = context.User.Claims.FirstOrDefault(c => c.Type == StringRoleResources.RoleClaim && c.Value == StringRoleResources.User);

                if (userRole != null && userRole.Value.Equals(StringRoleResources.User))
                {
                    context.Succeed(requirement);
                }
                else if (defaultRole != null && defaultRole.Value.Equals(StringRoleResources.Default))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
