

namespace SharedUI.AuthPolicies
{

    public class CustomerRoleRequirement : IAuthorizationRequirement
    {

        public CustomerRoleRequirement()
        {
        }


        public CustomerRoleRequirement(string role)
        {
            this.Role = role;
        }

        public string? Role { get; }
    }
}
