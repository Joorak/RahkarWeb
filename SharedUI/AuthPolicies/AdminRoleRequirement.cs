

namespace SharedUI.AuthPolicies
{

    public class AdminRoleRequirement : IAuthorizationRequirement
    {

        public AdminRoleRequirement(string role)
        {
            this.Role = role;
        }

        public string Role { get; }
    }
}
