using Application.Models;
using Domain.Entities.Identity;

namespace Application.Interfaces; 
public interface IRoleService {
    Task<List<string>> CheckUserRolesAsync(User user);

   
    RoleResponse? GetDefaultRole();


    RoleResponse? GetUserRole();

    RoleResponse? GetAdminRole();

    
    Task<RequestResponse> SetUserRoleAsync(User user, string role);


    Task<Role?> FindRoleByIdAsync(int roleId);



    Task<Role?> FindRoleByNameAsync(string name);

    Task<RequestResponse> CreateRoleAsync(CreateRoleRequest role);


    Task<RequestResponse> UpdateRoleAsync(UpdateRoleRequest role);

    Task<RequestResponse> DeleteRoleAsync(int roleId);

 
    List<RoleResponse> GetRoles();


    List<RoleResponse> GetRolesForAdmin();

    
    RoleResponse? GetRoleById(int id);

    RoleResponse? GetRoleByNormalizedName(string normalizedName);
} 
