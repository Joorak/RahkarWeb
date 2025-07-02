using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SharedUI.Interfaces
{
    public interface IAccessTokenService
    {
        Task<string> GetToken();
        Task SetToken(string token);
        Task RemoveToken();
        Task<string> GetItemAsync(string tokenName);
        string GetAccessToken(string tokenName);
        Task SetItemAsync(string tokenName, string tokenValue);
        Task RemoveItemAsync(string tokenName);
    }
}
