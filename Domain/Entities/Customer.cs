using Domain.Enums;
namespace Domain.Entities; 
public class Customer : BaseEntity { 
    public string FirstName { get; set; } = null!; 
    public string LastName { get; set; } = null!; 
    public string Email { get; set; } = null!; 
    public int BirthDate { get; set; } 
    public PersonGender Gender { get; set; } 
} 
