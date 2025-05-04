using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SharedUI.Services
{

    public class LocalStorageService : ILocalStorageService, IDisposable
    {
        private IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jSRuntime)
        {
            _jsRuntime = jSRuntime;
        }

        public async Task<string> GetItemAsync(string tokenName)
        {
            var res = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", tokenName);
            return res;
        }
        public string GetAccessToken(string tokenName)
        {

            return Task.Run(() => _jsRuntime.InvokeAsync<string>("localStorage.getItem", tokenName)).GetAwaiter().GetResult().Result;
        }
        public async Task SetItemAsync(string tokenName, string tokenValue)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", tokenName, tokenValue);
        }

        public async Task RemoveItemAsync(string tokenName)
        {
            await _jsRuntime.InvokeAsync<string>("localStorage.removeItem", tokenName);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

        }
    }
}
