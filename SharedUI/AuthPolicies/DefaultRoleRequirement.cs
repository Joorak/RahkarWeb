

namespace SharedUI.AuthPolicies
{

    public class DefaultRoleRequirement : IAuthorizationRequirement
    {

        public DefaultRoleRequirement(string role)
        {
            this.Role = role;
        }

        public string Role { get; }
    }
}
