using Domain.Entities.Identity; 
using FluentValidation; 
namespace Application.Validators; 
public interface IUserValidator<User> {
    Task<bool> IsUsernameUnique(string email);
} 
