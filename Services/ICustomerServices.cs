using Bank_Project.DTOs;
using Bank_Project.Models;

public interface ICustomerServices
{
    Task<Customers> AddCustomerOnlyAsync(CreateCustomerDto customerDto);
    Task<Customers?> GetCustomerByIdAsync(int id);
    Task<List<Customers>> GetAllCustomersAsync();
    Task<Customers?> UpdateCustomerAsync(int id, Customers customer);
    Task<bool> DeleteCustomerAsync(int id);
}