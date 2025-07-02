using Application.Models;
using Domain.Entities.Identity;
using SharedUI.Extensions;
using Application.Interfaces;
using System.Threading.Tasks;

namespace SharedUI.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly IAccessTokenService _tokenService;
        private readonly NavigationManager _navigationManager;

        public AccountService(HttpClient httpClient, IAccessTokenService tokenService, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _navigationManager = navigationManager;
        }

        public async Task<RequestResponse> ChangePasswordUserAsync(ChangePasswordRequest command)
        {
            var token = await _tokenService.GetToken();
           
            return await _httpClient.PostAsync<RequestResponse>(ApiEndpoint.Account.ChangePassword, command, token);
        }

        public async Task<RequestResponse> ResetPasswordUserAsync(ResetPasswordRequest command)
        {
            var token = await _tokenService.GetToken();
            return await _httpClient.PostAsync<RequestResponse>(ApiEndpoint.Account.ResetPassword, command, token);
        }

        public async Task<RequestResponse<JwtTokenResponse>> LoginAsync(LoginRequest login)
        {
            return await _httpClient.PostAsync<RequestResponse<JwtTokenResponse>>(ApiEndpoint.Account.Login, login, null);
        }

        public async Task<RequestResponse> ValidateToken(string token)
        {
            return await _httpClient.GetAsync<RequestResponse>(ApiEndpoint.Account.ValidateToken, token);
        }

        public Task<RequestResponse<JwtTokenResponse>> GenerateToken(JwtTokenRequest jwtTokenRequest)
        {
            throw new NotImplementedException();
        }

        public Task<RequestResponse<JwtTokenResponse>> RegisterAsync(RegisterRequest register)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            throw new NotImplementedException();
        }
    }
}