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

        public async Task<Accounts> CreateAccountAsync(CreateAccountDto accountDto, Customers customer, Branches branch)
        {
            try
            {
                var account = new Accounts
                {
                    AccountType = accountDto.AccountType,
                    Balance = accountDto.Balance,
                    Branches = branch,
                    Customers = customer,  // This links the Account to the Customer automatically
                    openDate = DateTime.UtcNow
                };

                var hasher = new PasswordHasher<Accounts>();
                account.PasswordHashed = hasher.HashPassword(account, accountDto.PasswordHashed);

                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync(); // This should now save the customer and the account

                return account;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating account for customer {CustomerId}", customer.CUID);
                throw new Exception("An error occurred while saving the entity changes. See the inner exception for details.", ex);
            }
        }

        public async Task<Accounts> CreateFullAccountAsync(CreateAccountDto accountDto, Customers customer, Branches branch, CreateCardDto cardDto)
        {
            var account = await CreateAccountAsync(accountDto, customer, branch);

            await _cardService.CreateCardAsync(account, cardDto);

            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Accounts> GetPrimaryAccountAsync(int customerId)
        {
            var account = await _context.Accounts
                .Include(a => a.Cards)
                .Include(a => a.Customers)
                .Include(a => a.Branches)
                .FirstOrDefaultAsync(a => a.Customers.CUID == customerId);

            if (account == null)
                throw new Exception($"Customer with ID {customerId} has no account.");

            return account;
        }

        public async Task IncreaseBalanceAsync(int customerId, int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than 0");

            var account = await GetPrimaryAccountAsync(customerId);
            account.Balance += amount;
            await _context.SaveChangesAsync();
        }

        public async Task AdjustBalanceAsync(int customerId, int diff)
        {
            var account = await GetPrimaryAccountAsync(customerId);
            account.Balance += diff;
            await _context.SaveChangesAsync();
        }
        public async Task<Branches> GetBranchByIdAsync(int branchId)
        {
            var branch = await _context.Branches.FindAsync(branchId);
            if (branch == null)
                throw new Exception($"Branch with ID {branchId} not found.");
            return branch;
        }
    }
}  