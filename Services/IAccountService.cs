using Bank_Project.DTOs;
using Bank_Project.Models;
using Microsoft.Identity.Client;

namespace Bank_Project.Services
{
     public interface IAccountService
    {
        Task<AccountRespDto?> GetPrimaryAccountAsync(int customerId);
        Task DepositAsync(int customerId, int amount, int accountid);
        Task WithdrawAsync(int customerId, int amount, int accountid);
        Task TransferAsync(int fromCus, int fromAccountId, int toCus, int toAccountId, int amount);


    }
}