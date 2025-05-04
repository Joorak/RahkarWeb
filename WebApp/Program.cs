using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using SharedUI;



var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddSharedUI(builder.Configuration);


//builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
//builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
//builder.Services.AddBootstrapBlazor(options =>
//{
//    // Uniformly set the automatic disappearance time of the Toast component
//    options.ToastDelay = 4000;
//});
var app = builder.Build();
#pragma warning disable CRR0029 // ConfigureAwait(true) is called implicitly
await app.RunAsync();
#pragma warning restore CRR0029 // ConfigureAwait(true) is called implicitly
