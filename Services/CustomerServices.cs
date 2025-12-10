using Bank_Project.Data;
using Bank_Project.Models;
using Bank_Project.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bank_Project.Services
{

    public class CustomerServices : ICustomerServices

    {
        private readonly AppDbContext _context;

        public CustomerServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customers> AddCustomerAsync(CreateCustomerDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));


            var branch = await _context.Branches.FindAsync(dto.BranchID);
            if (branch == null)
                throw new Exception("Branch not found");

            var customer = new Customers
            {
                FirstName = dto.FirstName,
                SecondName = dto.SecondName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Salary = dto.Salary
            };

            var account = new Accounts
            { 
                AccountType= dto.AccountType,
                Balance= dto.Balance,
                openDate=DateTime.Now,
                BranchID = dto.BranchID,
                Branches = branch,
                Customers = customer,
                Cards = null!
            };

            var card = new Cards
            {
                CardNumber = Cards.GenerateCardNumber(),
                CardType = dto.CardType,
                ExpiryDate = DateTime.Now.AddYears(5),
                CVV = Cards.GenerateCVV(),
                Account = account 
            };

            account.Cards = card;

           
            var accountHasher = new PasswordHasher<Accounts>();
            account.PasswordHashed = accountHasher.HashPassword(account, dto.Password);

            var cardHasher = new PasswordHasher<Cards>();
            card.PasswordHash = cardHasher.HashPassword(card, dto.CardPassword);

            _context.Customers.Add(customer);
            _context.Accounts.Add(account);
            _context.Cards.Add(card);

            await _context.SaveChangesAsync();

            return customer;
        }
        public async Task<Customers?> UpdateCustomerAsync(int id, Customers customer)
        {
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Customer with id {id} not found.");

            existing.FirstName = customer.FirstName;
            existing.SecondName = customer.SecondName;
            existing.LastName = customer.LastName;
            existing.PhoneNumber = customer.PhoneNumber;
            existing.Address = customer.Address;
            existing.Salary = customer.Salary;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null)
                return false;

            _context.Customers.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Customers>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }
        public async Task<Customers?> GetCustomerByIdAsync(int id)
        {
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Customer with id {id} not found.");

            return existing;


        }

    }

}