﻿

namespace SharedUI.Interfaces
{

    public interface IRoleService
    {

        Task<List<RoleResponse>> GetRoles();


        Task<List<RoleResponse>> GetRolesForAdmin();


        Task<RoleResponse> GetRole(int id);


        Task<RequestResponse> AddRole(RoleResponse role);


        Task<RequestResponse> UpdateRole(RoleResponse role);


        Task<RequestResponse> DeleteRole(int id);
    }
}
