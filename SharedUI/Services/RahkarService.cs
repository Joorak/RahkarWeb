


using SharedUI.Extensions;
using SharedUI.Interfaces;
using System.Net.Http;

namespace SharedUI.Services
{
    public interface IRahkarService
    {

        Task<RequestResult<string>> GetConnectionString();
    }
    public class RahkarService : IRahkarService
    {
        private readonly HttpClient _httpClient;
        private readonly IAccessTokenService _accessTokenService;
        public RahkarService(HttpClient httpClient,
            IAccessTokenService accessTokenService)
        {
            _httpClient = httpClient;
            _accessTokenService = accessTokenService;
        }


        public async Task<RequestResult<string>> GetConnectionString()
        {
            var jwtToken = await _accessTokenService.GetItemAsync("jwt_token");
            var serviceResponse = await _httpClient.GetAsync<RequestResult<string>>($"Rahkar/GetConnectionString", jwtToken);
            return serviceResponse;
        }
    }
}
