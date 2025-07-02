

namespace SharedUI.Interfaces
{

    public interface IUserService
    {

        Task<List<UserResponse>> GetUsers();

        Task<List<UserResponse>> GetUsersInactive();

        Task<UserResponse> GetUser(int id);


        Task<RequestResponse> ActivateUser(ActivateUserRequest user);


        Task<RequestResponse> AddUser(CreateAccountRequest user);


        Task<RequestResponse> UpdateUser(UpdateUserRequest user);


        Task<RequestResponse> UpdateUserEmail(UpdateUserEmailRequest user);


        Task<RequestResponse> DeleteUser(int id);
    }
}
