using ServerApp.Pages;
using SharedUI;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel();
builder.WebHost.UseIISIntegration();
//builder.WebHost.SuppressStatusMessages(true);
builder.WebHost.UseUrls(builder.Configuration["HostURL"]!.ToString());


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


builder.Services.AddSharedUI(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.UseStaticFiles();
//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


//app.Run();
await app.StartAsync();
Console.WriteLine($"Application has started at : {string.Join(", ", app.Urls)}");
await app.WaitForShutdownAsync();
