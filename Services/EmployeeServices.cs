using Bank_Project.DTOs;
using Bank_Project.Data;
using Bank_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bank_Project.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly AppDbContext _context;

        public EmployeeServices(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Loans>GiveLoanAsync(CreateLoanDto dto)
        {


            ValidateLoanDto(dto);  
            
            var customer = await _context.Customers.FindAsync(dto.CustomerId);

            if (customer == null)
                 throw new Exception("Customer not found");
 
            


            var emp = await _context.Employees.FindAsync(dto.EmpID);

            if (emp == null)
                 throw new Exception("Employee not found");


            var loan = new Loans
            {
                CUID = dto.CustomerId,
                Customer = customer,
                Employees=emp,
                LoanStatus= dto.LoanStatus,
                LoanType= dto.LoanType,
                LoanAmount= dto.LoanAmount,
                PaymentAmount= dto.PaymentAmount,
                InterestRate = dto.InterestRate,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
            };

            
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            
            return loan;
        }

        private void ValidateLoanDto(CreateLoanDto dto)
        {
            if (dto == null) 
                throw new ArgumentNullException(nameof(dto));

            if (dto.LoanAmount <= 0)
                throw new ArgumentException("Loan amount must be greater than 0", nameof(dto.LoanAmount));

            if (dto.PaymentAmount <= 0)
                throw new ArgumentException("Payment amount must be greater than 0", nameof(dto.PaymentAmount));

            if (dto.InterestRate <= 0)
                throw new ArgumentException("Interest rate must be greater than 0", nameof(dto.InterestRate));
        }
        
    }
}