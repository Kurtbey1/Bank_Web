using Bank_Project;
using Bank_Project.Data;
using Bank_Project.DTOs;
using Bank_Project.Models;
using Bank_Project.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class BankCoordinatorService
{
    
    private readonly ICustomerServices _customerService;
    private readonly IAccountService _accountService;
    private readonly CardService _cardService;
    private readonly AppDbContext _context;
    private readonly ILogger<BankCoordinatorService> _logger;

    public BankCoordinatorService(ICustomerServices customerService, IAccountService accountService, CardService cardService, AppDbContext context, ILogger<BankCoordinatorService> logger)
    {
        _customerService = customerService;
        _accountService = accountService;
        _cardService = cardService;
        _context = context;
        _logger = logger;
        _cardService = cardService;
        _context = context;
    }

    public async Task<Customers> AddCustomerWithAccountAsync(
        
    CreateCustomerDto customerDto,
    CreateAccountDto accountDto,
    CreateCardDto cardDto)
    {
        _logger.LogInformation("Starting process to add customer with account and card.");
        if (customerDto == null)
        {
            _logger.LogError("Customer DTO is null."); 
            throw new ArgumentNullException(nameof(customerDto));
        }
        if (accountDto == null){ 
            _logger.LogError("Account DTO is null.");
        throw new ArgumentNullException(nameof(accountDto));
        }
        if (cardDto == null) {
        throw new ArgumentNullException(nameof(cardDto));
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var customer = await _customerService.AddCustomerOnlyAsync(customerDto);

            var branch = await _context.Branches.FindAsync(accountDto.BranchId);
            if (branch == null){
                _logger.LogWarning("Branch with ID {BranchId} not found", accountDto.BranchId);
                throw new Exception($"Branch with ID {accountDto.BranchId} not found.");
            }
            var account = new Accounts
            {
                AccountType = accountDto.AccountType,
                Balance = accountDto.Balance,
                Branches = branch,
                Customer = customer,
                openDate = DateTime.UtcNow
            };

            var hasher = new PasswordHasher<Accounts>();
            account.PasswordHashed = hasher.HashPassword(account, accountDto.PasswordHashed);

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync(); // Save account to generate AccountID

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
            _logger.LogInformation("Successfully added customer with account and card. Customer ID: {CustomerId}", customer.CUID);
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