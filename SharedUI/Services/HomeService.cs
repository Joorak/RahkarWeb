


using Domain.Entities.Identity;
using Domain.OldEntities;
using SharedUI.Extensions;
using SharedUI.Interfaces;
using System.Net.Http;

namespace SharedUI.Services
{
    public interface IHomeService
    {

        Task<RequestResult<CountriesTurnoverStat>> GetCountriesTurnover();
    }
    public class HomeService : IHomeService
    {
        private readonly HttpClient _httpClient;
        private readonly IAccessTokenService _accessTokenService;
        public HomeService(HttpClient httpClient,
            IAccessTokenService accessTokenService)
        {
            _httpClient = httpClient;
            _accessTokenService = accessTokenService;
        }


        public async Task<RequestResult<CountriesTurnoverStat>> GetCountriesTurnover()
        {
            var jwtToken = await _accessTokenService.GetItemAsync("jwt_token");
            var serviceResponse = await _httpClient.GetAsync<RequestResult<CountriesTurnoverStat>>($"getExcelData", jwtToken);
            return serviceResponse;
            //var response = await this._httpClient.GetAsync($"Home/getExcelData");
            //var responseResult = await response.Content.ReadAsStringAsync();

            //var result = JsonSerializer.Deserialize<RequestResult<CountriesTurnoverStat>>(
            //        responseResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //return result!;
        }
    }
}
