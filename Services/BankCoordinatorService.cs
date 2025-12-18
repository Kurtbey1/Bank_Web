using Bank_Project;
using Bank_Project.Data;
using Bank_Project.DTOs;
using Bank_Project.Models;
using Bank_Project.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class BankCoordinatorService
{
    
    private readonly CustomerServices _customerService;
    private readonly AccountService _accountService;
    private readonly CardService _cardService;
    private readonly AppDbContext _context;

    public BankCoordinatorService(CustomerServices customerService, AccountService accountService, CardService cardService, AppDbContext context)
    {
        _customerService = customerService;
        _accountService = accountService;
        _cardService = cardService;
        _context = context;
    }

    public async Task<Customers> AddCustomerWithAccountAsync(
    CreateCustomerDto customerDto,
    CreateAccountDto accountDto,
    CreateCardDto cardDto)
    {
        if (customerDto == null) throw new ArgumentNullException(nameof(customerDto));
        if (accountDto == null) throw new ArgumentNullException(nameof(accountDto));
        if (cardDto == null) throw new ArgumentNullException(nameof(cardDto));

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1️⃣ Create customer
            var customer = await _customerService.AddCustomerOnlyAsync(customerDto);

            // 2️⃣ Get branch
            var branch = await _context.Branches.FindAsync(accountDto.BranchId);
            if (branch == null)
                throw new Exception($"Branch with ID {accountDto.BranchId} not found.");

            // 3️⃣ Create account using Customer and Branch objects
            var account = new Accounts
            {
                AccountType = accountDto.AccountType,
                Balance = accountDto.Balance,
                Branches = branch,
                Customers = customer,
                openDate = DateTime.UtcNow
            };

            var hasher = new PasswordHasher<Accounts>();
            account.PasswordHashed = hasher.HashPassword(account, accountDto.PasswordHashed);

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync(); // Save account to generate AccountID

            // 4️⃣ Create card using the Account object
            var card = new Cards
            {
                CardType = cardDto.CardType,
                Account = account,
                CVV = Cards.GenerateCVV(),
                ExpiryDate = DateTime.UtcNow.AddYears(5),
                CardNumber = Cards.GenerateCardNumber()
            };

            var cardHasher = new PasswordHasher<Cards>();
            card.PasswordHash = cardHasher.HashPassword(card, cardDto.CardPassword);

            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync(); // Save card

            await transaction.CommitAsync();
            return customer;
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            // Log the inner exception for debugging
            throw new Exception($"Database update failed: {ex.InnerException?.Message ?? ex.Message}");
        }
    }


}