using Application.Models;
using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public interface IExternalApiService
    {
        Task<T?> GetAsync<T>(string url);
        Task<RequestResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest payload) where TResponse : class;
    }

    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }

        public async Task<RequestResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest payload) where TResponse : class
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<TResponse>(stream);
                return RequestResponse<TResponse>.Success(result);
            }
            catch (Exception)
            {
                //_logger.LogError(ex, "Error sending SMS code to: {MobileNumber}", mobileNumber);
                return RequestResponse<TResponse>.Failure($"Error sending requet to {url}");
                
            }


            
        }

       
    }
}
