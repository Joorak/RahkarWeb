


namespace SharedUI.AuthPolicies
{

    public class UserRoleRequirement : IAuthorizationRequirement
    {

        public UserRoleRequirement(string role)
        {
            this.Role = role;
        }

        public string Role { get; }
    }
}
