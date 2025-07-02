namespace SharedUI.Services
{
    public class UserService : IUserService
    {
        public UserService(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
            this.Options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            //this.SnackBar = snackBar;
        }

        private HttpClient HttpClient { get; }



        private JsonSerializerOptions Options { get; }

        public async Task<RequestResponse> AddUser(CreateAccountRequest user)
        {
            var data = new CreateAccountRequest
            {
                AccountId = user.AccountId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
            };

            var response = await this.HttpClient.PostAsJsonAsync($"Users/user", data);
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The user was added.", Severity.Success);
            //}

            return result;
        }

        public async Task<RequestResponse> ActivateUser(ActivateUserRequest user)
        {
            var data = new ActivateUserRequest
            {
                Id = user.Id,
            };

            var response = await this.HttpClient.PostAsJsonAsync($"Users/userActivate", data);
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The user was activated.", Severity.Success);
            //}

            return result;
        }

        public async Task<RequestResponse> DeleteUser(int id)
        {
            var response = await this.HttpClient.DeleteAsync($"Users/user/{id}");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The user was deleted.", Severity.Success);
            //}

            return result;
        }

        public async Task<UserResponse> GetUser(int id)
        {
            var response = await this.HttpClient.GetAsync($"Users/user/{id}");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResult<UserResponse>>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}

            return !response.IsSuccessStatusCode
                ? null
                : result.Item;
        }

        public async Task<List<UserResponse>> GetUsers()
        {
            var response = await this.HttpClient.GetAsync("Users/users");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResult<UserResponse>>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}

            return !response.IsSuccessStatusCode
                ? null
                : result.Items;
        }

        public async Task<List<UserResponse>> GetUsersInactive()
        {
            var response = await this.HttpClient.GetAsync("Users/usersInactive");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResult<UserResponse>>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}

            return !response.IsSuccessStatusCode
                ? null
                : result.Items;
        }

        public async Task<RequestResponse> UpdateUser(UpdateUserRequest user)
        {
            var data = new UpdateUserRequest
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
            };

            var response = await this.HttpClient.PutAsJsonAsync($"Users/user", data);
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The user was updated.", Severity.Success);
            //}

            return result;
        }

        public async Task<RequestResponse> UpdateUserEmail(UpdateUserEmailRequest user)
        {
            var response = await this.HttpClient.PutAsJsonAsync($"Users/userEmail", user);
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The user email address was updated.", Severity.Success);
            //}

            return result;
        }
    }
}
