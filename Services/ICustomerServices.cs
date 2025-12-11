using Bank_Project.DTOs;
using Bank_Project.Models;

namespace Bank_Project.Services
{
   public interface ICustomerServices
{
    Task<Customers> AddCustomerAsync(CreateCustomerDto customerDto, CreateAccountDto accountDto);
    Task<Customers?> UpdateCustomerAsync(int id, Customers customer);
    Task<bool> DeleteCustomerAsync(int id);
    Task<List<Customers>> GetAllCustomersAsync();
    Task<Customers?> GetCustomerByIdAsync(int id); 
}
}
