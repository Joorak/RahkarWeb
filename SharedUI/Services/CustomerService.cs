

using Application.Models;
using Domain.Entities;

namespace SharedUI.Services
{

    public class CustomerService : ICustomerService
    {

        public CustomerService(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
            this.Options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
        }


        private HttpClient HttpClient { get; }




        private JsonSerializerOptions Options { get; }


        public async Task<Result<Customer>> GetCustomers()
        {
            var response = await this.HttpClient.GetAsync($"Customers/Customers");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Result<Customer>>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}

            return !response.IsSuccessStatusCode ? null : result;
        }


        public async Task<Result<Customer>> GetCustomer(int id)
        {
            var response = await this.HttpClient.GetAsync($"Customers/Customer/{id}");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Result<Customer>>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result!.Error, MudBlazor.Severity.Error);
            //}

            return response.IsSuccessStatusCode ? null : result;
        }
        public async Task<Result<Customer>> GetCustomer(string userEmail)
        {
            var response = await this.HttpClient.GetAsync($"Customers/Customer/{userEmail}");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Result<Customer>>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result!.Error, MudBlazor.Severity.Error);
            //}

            return !response.IsSuccessStatusCode ? null : result;
        }

        public async Task<RequestResponse> AddCustomer(EntityRequest<Customer> Customer)
        {
            var response = await this.HttpClient.PostAsJsonAsync("Customers/Customer", Customer);
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The Customer was added.", MudBlazor.Severity.Info);
            //}

            return result;
        }


        public async Task<RequestResponse> UpdateCustomer(EntityRequest<Customer> Customer)
        {
            var response = await this.HttpClient.PutAsJsonAsync("Customers/Customer", Customer);
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The Customer was updated.", MudBlazor.Severity.Info);
            //}

            return result;
        }


        public async Task<RequestResponse> DeleteCustomer(int id)
        {
            var response = await this.HttpClient.DeleteAsync($"Customers/Customer/{id}");
            var responseResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RequestResponse>(
                responseResult, this.Options);

            //if (response.IsSuccessStatusCode == false)
            //{
            //    this.SnackBar.Add(result.Error, MudBlazor.Severity.Error);
            //}
            //else
            //{
            //    this.SnackBar.Add("The Customer was deleted.", MudBlazor.Severity.Info);
            //}

            return result;
        }
    }
}
