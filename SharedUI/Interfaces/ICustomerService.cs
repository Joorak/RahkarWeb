

using Domain.Entities;

namespace SharedUI.Interfaces
{

    public interface ICustomerService
    {

        Task<Result<Customer>> GetCustomers();


        Task<Result<Customer>> GetCustomer(int id);

        Task<Result<Customer>> GetCustomer(string userEmail);
        Task<RequestResponse> AddCustomer(EntityRequest<Customer> Customer);


        Task<RequestResponse> UpdateCustomer(EntityRequest<Customer> Customer);


        Task<RequestResponse> DeleteCustomer(int id);
    }
}
