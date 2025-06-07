

using Microsoft.AspNetCore.Components.Authorization;

namespace SharedUI.Auth
{

    public class AuthStateProvider : AuthenticationStateProvider
    {
        public AuthStateProvider(HttpClient httpClient, IAccessTokenService localStorage, NavigationManager navMagager)
        {
            this.HttpClient = httpClient;
            this.LocalStorage = localStorage;
            this.Anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            this.NavMagager = navMagager;
        }


        private HttpClient HttpClient { get; }


        private IAccessTokenService LocalStorage { get; }


        private AuthenticationState Anonymous { get; }


        private NavigationManager NavMagager { get; }

        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await this.LocalStorage.GetItemAsync("authToken");

            AuthenticationState? authenticationState;
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(this.HttpClient.DefaultRequestHeaders.Authorization?.Parameter))
            {
                this.NotifyUserLogout();
                authenticationState = this.Anonymous;
            }
            else
            {
                this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.ToString());
                authenticationState = new AuthenticationState(
                        new ClaimsPrincipal(new ClaimsIdentity(JwtTokenParser.ParseClaimsFromJwt(token), "jwtAuthType")));
            }

            return authenticationState;
        }

        public void NotifyUserAuthentication(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtTokenParser.ParseClaimsFromJwt(token), "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            this.NotifyAuthenticationStateChanged(authState);
        }


        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(this.Anonymous);
            this.NotifyAuthenticationStateChanged(authState);
        }
    }
}
