


namespace SharedUI.Auth;

public class AuthenticationService : IAuthenticationService
{

    public AuthenticationService(
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider,
        ILocalStorageService localStorage,
        ToastService toastService)
    {
        this.HttpClient = httpClient;
        this.AuthStateProvider = authStateProvider;
        this.LocalStorage = localStorage;
        this.ToastService = toastService;
    }


    private HttpClient HttpClient { get; }

    private AuthenticationStateProvider AuthStateProvider { get; }


    private ILocalStorageService LocalStorage { get; }


    private ToastService ToastService { get; }


    public async Task<JwtTokenResponse> Login(LoginRequest Request)
    {
        var data = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("Email", Request.Email),
            new KeyValuePair<string, string>("Password", Request.Password),
        });

        var response = await this.HttpClient.PostAsync("Accounts/login", data);
        var responseResult = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<JwtTokenResponse>(
            responseResult,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        JwtTokenResponse? jwtResponse = null;
        if (response.IsSuccessStatusCode == false)
        {
            await ToastService.Error(result.Error);
        }
        else if (result.AccessToken == null)
        {
            await ToastService.Error("Access Token is null");
        }
        else
        {
            await this.LocalStorage.SetItemAsync("authToken", result.AccessToken.ToString());
            ((AuthStateProvider)this.AuthStateProvider).NotifyUserAuthentication(result.AccessToken.ToString());

            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(result.Type, result.AccessToken.ToString());
            jwtResponse = new JwtTokenResponse { AccessToken = result.AccessToken.ToString() };
        }

        return jwtResponse;
    }

    public async Task<JwtTokenResponse> Register(RegisterRequest Request)
    {
        var data = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("Email", Request.Email),
            new KeyValuePair<string, string>("FirstName", Request.FirstName),
            new KeyValuePair<string, string>("LastName", Request.LastName),
            new KeyValuePair<string, string>("RoleName", Request.RoleName),
            new KeyValuePair<string, string>("Password", Request.Password),
            new KeyValuePair<string, string>("ConfirmPassword", Request.ConfirmPassword),
        });

        var response = await this.HttpClient.PostAsync("Accounts/register", data);
        var responseResult = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<JwtTokenResponse>(
            responseResult,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        JwtTokenResponse? jwtResponse = null;
        if (response.IsSuccessStatusCode == false)
        {
            await ToastService.Error(result.Error);
        }
        else if (result.AccessToken == null)
        {
            await ToastService.Error("Access Token is null");
        }
        else
        {
            await this.LocalStorage.SetItemAsync("authToken", result.AccessToken.ToString());
            ((AuthStateProvider)this.AuthStateProvider).NotifyUserAuthentication(result.AccessToken.ToString());

            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(result.Type, result.AccessToken.ToString());
            jwtResponse = new JwtTokenResponse { AccessToken = result.AccessToken.ToString() };
        }

        return jwtResponse;
    }

    public async Task Logout()
    {
        await this.LocalStorage.RemoveItemAsync("authToken");
        ((AuthStateProvider)this.AuthStateProvider).NotifyUserLogout();
        this.HttpClient.DefaultRequestHeaders.Authorization = null;
    }
}
