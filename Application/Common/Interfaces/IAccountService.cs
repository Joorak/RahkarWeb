using Application.Models;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces;
public interface IAccountService
{
    Task<RequestResponse> ResetPasswordUserAsync(ResetPasswordRequest resetPassword);

    Task<RequestResponse> ChangePasswordUserAsync(ChangePasswordRequest changePassword);

    Task<JwtTokenResponse> GenerateToken(User user);
    Task<JwtTokenResponse> LoginAsync(LoginRequest login);

    Task<JwtTokenResponse> RegisterAsync(RegisterRequest register);

    Task<bool> CheckPasswordAsync(User user, string password);
}
