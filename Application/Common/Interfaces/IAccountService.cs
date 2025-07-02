using Application.Models;
using Domain.Entities.Identity;
using System.Data.SqlTypes;

namespace Application.Interfaces;
public interface IAccountService
{
    Task<RequestResponse> ResetPasswordUserAsync(ResetPasswordRequest resetPassword);

    Task<RequestResponse> ChangePasswordUserAsync(ChangePasswordRequest changePassword);

    Task<RequestResponse<JwtTokenResponse>> GenerateToken(JwtTokenRequest jwtTokenRequest);
    Task<RequestResponse> ValidateToken(string token);
    Task<RequestResponse<JwtTokenResponse>> LoginAsync(LoginRequest login);

    Task<RequestResponse<JwtTokenResponse>> RegisterAsync(RegisterRequest register);

    Task<bool> CheckPasswordAsync(User user, string password);
}
