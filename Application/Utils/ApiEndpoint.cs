namespace Application.Utils;
public static class ApiEndpoint
{
    public static class Account
    {
        public const string Base = "/account";
        public const string Login = Base + "/login";
        public const string ValidateToken = Base + "/validate-token";
        public const string ChangePassword = Base + "/change-password";
        public const string ResetPassword = Base + "/reset-password";

        public static string GetVersionedEndpoint(string endpoint, string version = "v1")
        {
            return $"/{version}{endpoint}";
        }
    }
}
