using Bank_Project.Data;
using Bank_Project.DTOs;
using Bank_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bank_Project.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly AppDbContext _context;
        private readonly ICustomerValidatorService _validator;

        public CustomerServices(AppDbContext context, ICustomerValidatorService validator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _validator = validator;
        }

        public async Task<Customers> AddCustomerOnlyAsync(CreateCustomerDto customerDto)
        {
            _validator.ValidateCustomerDto(customerDto);

            var customer = new Customers
            {
                FirstName = customerDto.FirstName,
                SecondName = customerDto.SecondName,
                LastName = customerDto.LastName,
                PhoneNumber = customerDto.PhoneNumber,
                Address = customerDto.Address,
                Salary = customerDto.Salary,
                Email = customerDto.Email,
                Gender = customerDto.Gender
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customers?> UpdateCustomerAsync(int id, Customers customer)
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

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null) return false;

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
            if (existing == null) throw new KeyNotFoundException($"Customer with id {id} not found.");
            return existing;
        }

     
    }
}