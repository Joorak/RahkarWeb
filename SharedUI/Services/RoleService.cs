// <copyright file="RoleService.cs" company="Joorak Rezapour">
// Copyright (c) Joorak Rezapour. All rights reserved.
// </copyright>


namespace SharedUI.Services
{
    public class RoleService : IRoleService
    {
        public RoleService(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
            this.Options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        }

        /// <summary>
        /// Gets the instance of the <see cref="HttpClient"/> to use.
        /// </summary>
        private HttpClient HttpClient { get; }



        /// <summary>
        /// Gets the instance of the <see cref="JsonSerializerOptions"/> to use.
        /// </summary>
        private JsonSerializerOptions Options { get; }

        /// <inheritdoc/>
        public async Task<RequestResponse> AddRole(RoleResponse role)
        {
            var response = await this.HttpClient.PostAsJsonAsync($"Roles/role", role);
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The role was added.", Severity.Success);
            //}

            return result!;
        }

        /// <inheritdoc/>
        public async Task<RequestResponse> DeleteRole(int id)
        {
            var response = await this.HttpClient.DeleteAsync($"Roles/role/{id}");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The role was deleted.", Severity.Success);
            //}

            return result!;
        }

        /// <inheritdoc/>
        public async Task<RoleResponse> GetRole(int id)
        {
            var response = await this.HttpClient.GetAsync($"Roles/role/{id}");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResult<RoleResponse>>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}

            return !response.IsSuccessStatusCode ? null! : result!.Item!;
        }

        /// <inheritdoc/>
        public async Task<List<RoleResponse>> GetRoles()
        {
            var response = await this.HttpClient.GetAsync("Roles/roles");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResult<RoleResponse>>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}

            return !response.IsSuccessStatusCode ? null! : result!.Items!;
        }

        /// <inheritdoc/>
        public async Task<List<RoleResponse>> GetRolesForAdmin()
        {
            var response = await this.HttpClient.GetAsync("Roles/rolesAdmin");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResult<RoleResponse>>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}

            return !response.IsSuccessStatusCode ? null! : result!.Items!;
        }

        /// <inheritdoc/>
        public async Task<RequestResponse> UpdateRole(RoleResponse role)
        {
            var data = new UpdateRoleRequest
            {
                Id = role.Id,
                Name = role.Name,
            };
            var response = await this.HttpClient.PutAsJsonAsync($"Roles/role", data);
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The role was updated.", Severity.Success);
            //}

            return result!;
        }
    }
}
