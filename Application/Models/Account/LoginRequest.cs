

namespace Application.Models
{

    public class LoginRequest 
    {

        public string? AccountId { get; set; }
        public string RoleForLogin { get; set; }

        public string? PassKey { get; set; }
    }
}
