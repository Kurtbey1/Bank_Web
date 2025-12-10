using Bank_Project.Models;
using Bank_Project.DTOs;

namespace Bank_Project.Services
{
    public interface ICustomerServices
    {
        Task<List<Customers>> GetAllCustomersAsync();
        Task<Customers?> GetCustomerByIdAsync(int id);
        Task<Customers> AddCustomerAsync(CreateCustomerDto dto);
        Task<Customers?> UpdateCustomerAsync(int id, Customers customer);
        Task<bool> DeleteCustomerAsync(int id);
    }
}