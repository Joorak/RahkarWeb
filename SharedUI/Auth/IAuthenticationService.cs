

namespace SharedUI.Auth
{

    public interface IAuthenticationService
    {

        Task<RequestResponse<JwtTokenResponse>> Login(LoginRequest userForAuthenticatrion);

        Task<RequestResponse<JwtTokenResponse>> Register(RegisterRequest userForAuthenticatrion);


        Task Logout();
    }
}
