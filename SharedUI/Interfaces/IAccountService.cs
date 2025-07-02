using System.Threading.Tasks;

namespace SharedUI.Services
{
    public interface IAccountService_
    {
        Task<RequestResponse> ChangePassword(ChangePasswordRequest command);
        Task<RequestResponse> ResetPassword(ResetPasswordRequest command);
        Task<RequestResponse> Login(LoginRequest login);
        Task<RequestResponse> ValidateToken(string token); // متد جدید برای اعتبارسنجی
    }
}