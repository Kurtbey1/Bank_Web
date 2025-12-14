using Bank_Project.Models;

namespace Bank_Project.Services
{
     public interface IAccountService
    {
        Task<Accounts> GetPrimaryAccountAsync(int customerId);
        Task IncreaseBalanceAsync(int customerId, int amount);
        Task AdjustBalanceAsync(int customerId, int diff);
        Task<Accounts> CreateAccountAsync(CreateAccountDto accountDto, Customers customer, Branches branch);
    }
}