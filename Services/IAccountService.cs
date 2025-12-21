using Bank_Project.DTOs;
using Bank_Project.Models;

namespace Bank_Project.Services
{
     public interface IAccountService
    {
        Task<AccountRespDto?> GetPrimaryAccountAsync(int customerId);
        Task IncreaseBalanceAsync(int customerId, int amount);
        Task AdjustBalanceAsync(int customerId, int diff);


    }
}