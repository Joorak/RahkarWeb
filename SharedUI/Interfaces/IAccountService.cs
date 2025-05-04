

namespace SharedUI.Interfaces
{
    public interface IAccountService
    {
        Task<RequestResponse> ChangePassword(ChangePasswordRequest command);

        Task<RequestResponse> ResetPassword(ResetPasswordRequest command);
    }
}
