

namespace SharedUI.Services
{

    public class SessionStorageService
    {

        public SessionStorageService(IJSRuntime js)
        {
            this.Js = js;
            this.JsInProcess = (IJSInProcessRuntime)this.Js;
        }

        private IJSRuntime Js { get; }


        private IJSInProcessRuntime JsInProcess { get; }


        public async Task<T> GetItemAsync<T>(string key)
        {
            var json = await this.Js.InvokeAsync<string>("getSessionStorage",key);

            return string.IsNullOrEmpty(json) ? default! : JsonSerializer.Deserialize<T>(json)!;
        }


        public async Task SetItemAsync<T>(string key, T item)
        {
            await this.Js.InvokeVoidAsync("setSessionStorage", key, JsonSerializer.Serialize(item));
        }


        public void SetItem<T>(string key, T item)
        {
            this.JsInProcess.InvokeVoid( "setSessionStorage", key, JsonSerializer.Serialize(item));
        }
    }
}
