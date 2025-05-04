

namespace SharedUI.Auth
{

    public interface IAuthenticationService
    {

        Task<JwtTokenResponse> Login(LoginRequest userForAuthenticatrion);

        Task<JwtTokenResponse> Register(RegisterRequest userForAuthenticatrion);


        Task Logout();
    }
}
