using Application.Models;
using Domain.Entities.Identity;
using System.Data.SqlTypes;

namespace Application.Interfaces;
public interface IAccountService
{
    Task<RequestResponse> ResetPasswordUserAsync(ResetPasswordRequest resetPassword);

    Task<RequestResponse> ChangePasswordUserAsync(ChangePasswordRequest changePassword);

    //Task<RequestResponse<JwtTokenResponse>> GenerateTokenAsync(JwtTokenRequest jwtTokenRequest);
    Task<RequestResponse> ValidateTokenAsync(string token);
    Task<RequestResponse<JwtTokenResponse>> LoginAsync(LoginRequest login);
    Task<RequestResponse<JwtTokenResponse>> LoginAdminAsync(string accountId);
    Task<RequestResponse<JwtTokenResponse>> RegisterAsync(RegisterRequest register);
    Task<bool> SendPassKeyAsync(string mobileNumber, string passKey);
    Task<bool> CheckPasswordAsync(User user, string password);
}
