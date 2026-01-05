using Bank_Project.Data;
using Bank_Project.DTOs;
using Bank_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank_Project.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountService> _logger;

        public AccountService(AppDbContext context, ILogger<AccountService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task<Accounts> GetAccountEntityAsync(int customerId, int accountid)
        {
            Console.WriteLine(_context.Database.GetDbConnection().Database);

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountID == accountid && a.Customer.CUID == customerId);

            if (account == null)
            {
                _logger.LogWarning("Account fetch failed: Customer {Cid}, Account {Aid} not found", customerId, accountid);
                throw new KeyNotFoundException($"Account for Customer ID {customerId} not found.");
            }

            return account;
        }
        
        public async Task<AccountRespDto?> GetPrimaryAccountAsync(int customerId)
        {
            _logger.LogInformation("Fetching primary account for Customer {Id}", customerId);
            Console.WriteLine(_context.Database.GetDbConnection().Database);

            var account = await _context.Accounts
                .Include(a => a.Cards)
                .Include(a => a.Customer)
                .Include(a => a.Branches)
                .FirstOrDefaultAsync(a => a.Customer.CUID == customerId);

            if (account == null)
            {
                _logger.LogWarning("No account found for Customer {Id}", customerId);
                throw new Exception($"Customer with ID {customerId} has no account.");
            }

            return new AccountRespDto
            {
                AccountId = account.AccountID,
                Balance = account.Balance,
                AccountType = account.AccountType
            };
        }

        public async Task DepositAsync(int customerId, int accountid, int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than 0");

            _logger.LogInformation("Processing deposit: {Amount} to Account {Aid} for Customer {Cid}", amount, accountid, customerId);

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var account = await GetAccountEntityAsync(customerId, accountid);

                account.Balance += amount;

                var transactionEntry = new Transactions
                {
                    AccountID = account.AccountID,
                    Amount = amount,
                    TransactionType = "Deposit",
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(transactionEntry);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                _logger.LogInformation("Deposit successful: Account {Aid}, New Balance {Balance}", accountid, account.Balance);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError(ex, "Deposit failed: Customer {Cid}, Account {Aid}", customerId, accountid);
                throw;
            }
        }

        public async Task WithdrawAsync(int customerId, int accountid, int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than 0");

            _logger.LogInformation("Processing withdrawal: {Amount} from Account {Aid} for Customer {Cid}", amount, accountid, customerId);

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var account = await GetAccountEntityAsync(customerId, accountid);


                if (account.Balance < amount)
                {
                    _logger.LogWarning("Withdrawal denied: Insufficient funds for Account {Aid}. Balance: {Bal}, Requested: {Amt}",
                        accountid, account.Balance, amount);
                    throw new Exception("Insufficient balance");
                }

                account.Balance -= amount;

                var transactionEntry = new Transactions
                {
                    AccountID = account.AccountID,
                    Amount = -amount,
                    TransactionType = "Withdraw",
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(transactionEntry);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                _logger.LogInformation("Withdrawal successful: Account {Aid}, New Balance {Balance}", accountid, account.Balance);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError(ex, "Withdrawal failed: Customer {Cid}, Account {Aid}", customerId, accountid);
                throw;
            }
        }

        public async Task TransferAsync(int fromCus, int fromAccountId, int toCus, int toAccountId, int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than 0");

            _logger.LogInformation("Initiating transfer: {Amt} from Acc {Fid} to Acc {Tid}", amount, fromAccountId, toAccountId);

            if (fromAccountId == toAccountId)
                throw new InvalidOperationException("Source and Destination accounts must be different.");

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sender = await GetAccountEntityAsync(fromCus, fromAccountId);
                var receiver = await GetAccountEntityAsync(toCus, toAccountId);

                if (sender.Balance < amount)
                {
                    _logger.LogWarning("Transfer aborted: Insufficient funds in Source Account {Aid}", fromAccountId);
                    throw new InvalidOperationException("Insufficient balance");
                }

                sender.Balance -= amount;
                receiver.Balance += amount;

                var senderTx = new Transactions { AccountID = fromAccountId, Amount = -amount, TransactionType = "Transfer Out", CreatedAt = DateTime.UtcNow };
                var receiverTx = new Transactions { AccountID = toAccountId, Amount = amount, TransactionType = "Transfer In", CreatedAt = DateTime.UtcNow };

                await _context.Transactions.AddRangeAsync(senderTx, receiverTx);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                _logger.LogInformation("Transfer completed successfully from {Fid} to {Tid}", fromAccountId, toAccountId);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError(ex, "Transfer failed: From {Fid} to {Tid}", fromAccountId, toAccountId);
                throw;
            }
        }
    }
}