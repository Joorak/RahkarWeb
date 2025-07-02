using Application.Models;
using Domain.Entities.Identity;

namespace Application.Interfaces; 
public interface IUserService {
    Task<List<string>> GetUserRoleAsync(User user);

    List<UserResponse> GetUsers();


    List<UserResponse> GetUsersInactive();


    UserResponse? GetUserById(int userId);


    UserResponse? GetUserByEmail(string email);

    Task<User?> FindUserByIdAsync(int userId);

    Task<User?> FindUserByEmailAsync(string email);

    Task<RequestResponse> CreateUserAsync(CreateAccountRequest user);

    Task<RequestResponse> AssignUserToRoleAsync(AssignUserToRoleRequest user);

    Task<RequestResponse> UpdateUserAsync(UpdateUserRequest user);

    Task<RequestResponse> ActivateUserAsync(ActivateUserRequest user);

    Task<RequestResponse> UpdateUserEmailAsync(UpdateUserEmailRequest user);
    Task<RequestResponse> DeleteUserAsync(int userId);

    
    Task<bool> IsInRoleAsync(int userId, string role);

    Task<bool> AuthorizeAsync(int userId, string policyName);
} 
