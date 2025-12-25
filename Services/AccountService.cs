using Bank_Project.Data;
using Bank_Project.DTOs;
using Bank_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Bank_Project.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly CardService _cardService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(AppDbContext context, CardService cardService, ILogger<AccountService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cardService = cardService ?? throw new ArgumentNullException(nameof(cardService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AccountRespDto?> GetPrimaryAccountAsync(int customerId)
        {
            var account = await _context.Accounts
                .Include(a => a.Cards)
                .Include(a => a.Customers)
                .Include(a => a.Branches)
                .FirstOrDefaultAsync(a => a.Customers.CUID == customerId);

            if (account == null)
                throw new Exception($"Customer with ID {customerId} has no account.");

            return new AccountRespDto
            {
                AccountId = account.AccountID,
                Balance = account.Balance,
                AccountType = account.AccountType
            };
        }

        public async Task IncreaseBalanceAsync(int customerId, int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than 0");

            var account = await GetPrimaryAccountAsync(customerId);
            if (account==null)
                throw new Exception($"Customer with ID {customerId} not found.");

            account.Balance += amount;
            await _context.SaveChangesAsync();
        }

        public async Task AdjustBalanceAsync(int customerId, int diff)
        {
            var account = await GetPrimaryAccountAsync(customerId);

            if (account == null)
                throw new Exception($"Customer with ID {customerId} has no account.");

            account.Balance += diff;
            await _context.SaveChangesAsync();
        }
       
    }
}  