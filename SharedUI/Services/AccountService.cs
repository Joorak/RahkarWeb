


namespace SharedUI.Services
{
    /// <summary>
    /// An implementation of <see cref="IAccountService"/>.
    /// </summary>
    public class AccountService : IAccountService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="httpClient">The instance of the <see cref="HttpClient"/> to use.</param>
        /// <param name="snackBar">The instance of the <see cref="ISnackbar"/> to use.</param>
        public AccountService(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
            this.Options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        /// <summary>
        /// Gets the instance of the <see cref="HttpClient"/> to use.
        /// </summary>
        private HttpClient HttpClient { get; }

        /// <summary>
        /// Gets the instance of the <see cref="ISnackbar"/> to use.
        /// </summary>

        /// <summary>
        /// Gets the instance of the <see cref="JsonSerializerOptions"/> to use.
        /// </summary>
        private JsonSerializerOptions Options { get; }

        /// <inheritdoc/>
        public async Task<RequestResponse> ChangePassword(ChangePasswordRequest request)
        {
            var response = await this.HttpClient.PutAsJsonAsync("Accounts/change-password", request);
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The password was changed.", Severity.Success);
            //}

            return result;
        }

        /// <inheritdoc/>
        public async Task<RequestResponse> ResetPassword(ResetPasswordRequest request)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", request.Email!),
                new KeyValuePair<string, string>("NewPassword", request.NewPassword!),
                new KeyValuePair<string, string>("NewConfirmPassword", request.NewConfirmPassword!),
            });

            var response = await this.HttpClient.PostAsync("Accounts/reset-password", data);
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The password was reset.", Severity.Success);
            //}

            return result;
        }
    }
}
