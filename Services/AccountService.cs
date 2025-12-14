// ===== AccountService.cs =====
using Bank_Project.DTOs;
using Bank_Project.Models;
using Microsoft.AspNetCore.Identity;
using Bank_Project.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Bank_Project.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly CardService _cardService;

        public AccountService(AppDbContext context, CardService cardService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cardService = cardService ?? throw new ArgumentNullException(nameof(cardService));
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
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0", nameof(amount));

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

        public async Task<Accounts> CreateAccountAsync(CreateAccountDto accountDto, Customers customer, Branches branch)
        {
            var account = new Accounts
            {
                AccountType = accountDto.AccountType,
                Balance = accountDto.Balance,
                Branches = branch,
                Customers = customer,
                openDate = DateTime.UtcNow
            };
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            var card = _cardService.CreateCard(account, new CreateCardDto
            {
                CardType = accountDto.CardType,
                CardPassword = accountDto.CardPassword
            });

            account.Cards = card;
            account.PasswordHashed = card.PasswordHash;
            await _context.SaveChangesAsync();

            return account;
        }
    }
}
