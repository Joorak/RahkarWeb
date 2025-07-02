

namespace Application.Models
{
    public class RegisterRequest 
    {

        public string? AccountId { get; set; }


        public string? FirstName { get; set; }

        public string? LastName { get; set; }


        public string? RoleForRegister { get; set; }


        public string? Password { get; set; }


        public string? ConfirmPassword { get; set; }
    }
}
