


namespace SharedUI.Auth;

public class AuthenticationService : IAuthenticationService
{

    public AuthenticationService(
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider,
        IAccessTokenService localStorage,
        ToastService toastService)
    {
        this.HttpClient = httpClient;
        this.AuthStateProvider = authStateProvider;
        this.LocalStorage = localStorage;
        this.ToastService = toastService;
    }


    private HttpClient HttpClient { get; }

    private AuthenticationStateProvider AuthStateProvider { get; }


    private IAccessTokenService LocalStorage { get; }


    private ToastService ToastService { get; }


    public async Task<RequestResponse<JwtTokenResponse>> Login(LoginRequest Request)
    {
        var data = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("Email", Request.AccountId!),
            new KeyValuePair<string, string>("Password", Request.PassKey!),
        });

        var response = await this.HttpClient.PostAsync("Accounts/login", data);
        var responseResult = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<RequestResponse<JwtTokenResponse>>(
            responseResult,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        JwtTokenResponse? jwtResponse = null;
        if (response.IsSuccessStatusCode == false)
        {
            await ToastService.Error(result!.Error!.ToString());
        }
        else if (result?.Item?.AccessToken == null)
        {
            await ToastService.Error("Access Token is null");
        }
        else
        {
            await this.LocalStorage.SetItemAsync("authToken", result.Item.AccessToken.ToString());
            ((AuthStateProvider)this.AuthStateProvider).NotifyUserAuthentication(result.Item.AccessToken.ToString());

            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(result?.Item?.Type!, result?.Item?.AccessToken.ToString());
            jwtResponse = new JwtTokenResponse { AccessToken = result?.Item?.AccessToken.ToString() };
        }

        return new RequestResponse<JwtTokenResponse>
        {

            Successful = true,

            Item = jwtResponse
        };
    }

    public async Task<RequestResponse<JwtTokenResponse>> Register(RegisterRequest Request)
    {
        var data = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("Email", Request.AccountId!),
            new KeyValuePair<string, string>("FirstName", Request.FirstName!),
            new KeyValuePair<string, string>("LastName", Request.LastName!),
            new KeyValuePair<string, string>("RoleName", Request.RoleForRegister!),
            new KeyValuePair<string, string>("Password", Request.Password!),
            new KeyValuePair<string, string>("ConfirmPassword", Request.ConfirmPassword!),
        });

        var response = await this.HttpClient.PostAsync("Accounts/register", data);
        var responseResult = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<RequestResponse<JwtTokenResponse>>(
            responseResult,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        JwtTokenResponse? jwtResponse = null;
        if (response.IsSuccessStatusCode == false)
        {
            await ToastService.Error(result?.Error!);
        }
        else if (result?.Item?.AccessToken == null)
        {
            await ToastService.Error("Access Token is null");
        }
        else
        {
            await this.LocalStorage.SetItemAsync("authToken", result.Item.AccessToken.ToString());
            ((AuthStateProvider)this.AuthStateProvider).NotifyUserAuthentication(result.Item.AccessToken.ToString());

            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(result.Item.Type!, result?.Item?.AccessToken.ToString());
            jwtResponse = new JwtTokenResponse { AccessToken = result?.Item?.AccessToken.ToString() };
        }

        return new RequestResponse<JwtTokenResponse>
        {

            Successful = true,

            Item = jwtResponse
        };
    }

    public async Task Logout()
    {
        await this.LocalStorage.RemoveItemAsync("authToken");
        ((AuthStateProvider)this.AuthStateProvider).NotifyUserLogout();
        this.HttpClient.DefaultRequestHeaders.Authorization = null;
    }
}
