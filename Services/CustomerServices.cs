using Bank_Project.Data;
using Bank_Project.DTOs;
using Bank_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Bank_Project.Services
{
    public class CustomerServices: ICustomerServices
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher < Accounts > _accountHasher = new();
        private readonly PasswordHasher < Cards > _cardHasher = new();
        public CustomerServices(AppDbContext context)
        {
            _context = context;
        }
        public async Task < Customers > AddCustomerAsync(CreateCustomerDto customerDto, CreateAccountDto accountDto)
        {
            ValidateCustomerDto(customerDto);
            ValidateAccountDto(accountDto);
            var branch = await _context.Branches.FindAsync(customerDto.BranchID);
            if (branch == null) throw new Exception("Branch not found");
            using
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var customer = CreateCustomerEntity(customerDto);
                await _context.Customers.AddAsync(customer);
                var account = CreateAccountEntity(accountDto, customer, branch);
                account.PasswordHashed = _accountHasher.HashPassword(account, accountDto.CardPassword);
                await _context.Accounts.AddAsync(account);
                var card = CreateCardEntity(account, accountDto);
                card.PasswordHash = _cardHasher.HashPassword(card, accountDto.CardPassword);
                await _context.Cards.AddAsync(card);
                account.Cards = card;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return customer;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task < Customers ? > UpdateCustomerAsync(int id, Customers customer)
        {
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null) throw new KeyNotFoundException($"Customer with id {id} not found.");
            existing.FirstName = customer.FirstName;
            existing.SecondName = customer.SecondName;
            existing.LastName = customer.LastName;
            existing.PhoneNumber = customer.PhoneNumber;
            existing.Address = customer.Address;
            existing.Salary = customer.Salary;
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task < bool > DeleteCustomerAsync(int id)
        {
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null) return false;
            _context.Customers.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task < List < Customers >> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }
        public async Task < Customers ? > GetCustomerByIdAsync(int id)
        {
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null) throw new KeyNotFoundException($"Customer with id {id} not found.");
            return existing;
        }
        // ========================= Helper Methods =========================
        private static Customers CreateCustomerEntity(CreateCustomerDto dto) => new()
        {
            FirstName = dto.FirstName,
                SecondName = dto.SecondName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Salary = dto.Salary
        };
        private static Accounts CreateAccountEntity(CreateAccountDto dto, Customers customer, Branches branch) => new()
        {
            AccountType = dto.AccountType,
                Balance = dto.Balance,
                openDate = DateTime.Now,
                BranchID = branch.BranchID,
                Branches = branch,
                Customers = customer,
                Cards = null!
        };
        private static Cards CreateCardEntity(Accounts account, CreateAccountDto dto) => new()
        {
            CardNumber = Cards.GenerateCardNumber(),
                CardType = dto.CardType,
                ExpiryDate = DateTime.Now.AddYears(5),
                CVV = Cards.GenerateCVV(),
                Account = account
        };
        private static void ValidateCustomerDto(CreateCustomerDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.FirstName)) throw new ArgumentException("First name required");
            if (string.IsNullOrWhiteSpace(dto.LastName)) throw new ArgumentException("Last name required");
            if (dto.Salary <= 0) throw new ArgumentException("Salary must be greater than 0");
        }
        private static void ValidateAccountDto(CreateAccountDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Balance < 0) throw new ArgumentException("Balance cannot be negative");
            if (string.IsNullOrWhiteSpace(dto.AccountType)) throw new ArgumentException("Account type required");
            if (string.IsNullOrWhiteSpace(dto.CardType)) throw new ArgumentException("Card type required");
            if (string.IsNullOrWhiteSpace(dto.CardPassword)) throw new ArgumentException("Card password required");
        }
    }
}