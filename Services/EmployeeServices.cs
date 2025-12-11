using Bank_Project.DTOs;
using Bank_Project.Data;
using Bank_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Bank_Project.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly AppDbContext _context;

        public EmployeeServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Loans> GiveLoanAsync(CreateLoanDto dto)
        {
            ValidateLoanDto(dto);

            var customer = await _context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.CUID == dto.CustomerId);

            if (customer == null)
                throw new Exception("Customer not found");

            var emp = await _context.Employees.FindAsync(dto.EmpID);
            if (emp == null)
                throw new Exception("Employee not found");

            var loan = new Loans
            {
                CUID = dto.CustomerId,
                Customer = customer,
                Employees = emp,
                LoanStatus = dto.LoanStatus,
                LoanType = dto.LoanType,
                LoanAmount = dto.LoanAmount,
                PaymentAmount = dto.PaymentAmount,
                InterestRate = dto.InterestRate,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
            };

            var account = customer.Accounts?.FirstOrDefault();
            if (account != null)
            {
                account.Balance += dto.LoanAmount;
            }

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<Loans> UpdateLoanDetailsAsync(int loanId, CreateLoanDto dto)
        {
            var existing = await _context.Loans
                .Include(l => l.Customer)
                    .ThenInclude(c => c.Accounts)
                .FirstOrDefaultAsync(l => l.LoanID == loanId);

            if (existing == null)
                throw new KeyNotFoundException($"Loan with ID {loanId} not found");

            ValidateLoanDto(dto);

            var emp = await _context.Employees.FindAsync(dto.EmpID);
            if (emp == null)
                throw new KeyNotFoundException($"Employee with ID {dto.EmpID} not found");

            var account = existing.Customer.Accounts?.FirstOrDefault();
            if (account != null)
            {
                int diff = dto.LoanAmount - existing.LoanAmount;
                account.Balance += diff;
            }

            existing.LoanStatus = dto.LoanStatus;
            existing.LoanType = dto.LoanType;
            existing.LoanAmount = dto.LoanAmount;
            existing.PaymentAmount = dto.PaymentAmount;
            existing.InterestRate = dto.InterestRate;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;
            existing.EmpID = dto.EmpID;
            existing.Employees = emp;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<Loans>> GetCustomersAllLoansAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.CUID == customerId);

            if (customer == null)
                throw new Exception("Customer Not Found");

            return await _context.Loans
                .Where(l => l.CUID == customerId)
                .ToListAsync();
        }

        private void ValidateLoanDto(CreateLoanDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Loan data cannot be null");

            if (string.IsNullOrWhiteSpace(dto.LoanStatus))
                throw new ArgumentException("Loan status is required", nameof(dto.LoanStatus));

            if (string.IsNullOrWhiteSpace(dto.LoanType))
                throw new ArgumentException("Loan type is required", nameof(dto.LoanType));

            if (dto.LoanAmount <= 0)
                throw new ArgumentException("Loan amount must be greater than 0", nameof(dto.LoanAmount));

            if (dto.PaymentAmount <= 0)
                throw new ArgumentException("Payment amount must be greater than 0", nameof(dto.PaymentAmount));

            if (dto.InterestRate <= 0)
                throw new ArgumentException("Interest rate must be greater than 0", nameof(dto.InterestRate));

            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("Start date must be earlier than end date", nameof(dto.StartDate));

            if (dto.EmpID <= 0)
                throw new ArgumentException("Employee ID must be a positive number", nameof(dto.EmpID));
        }


        public async Task<Accounts?> CheckAccountCustomersAsync(int customerId, int accountId)
        {
            var customer = await _context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.CUID == customerId);

            if (customer == null)
                throw new Exception($"Customer with ID {customerId} not found");

          
            var account = customer.Accounts?.FirstOrDefault(a => a.AccountID == accountId);

            if (account == null)
                throw new Exception($"Account with ID {accountId} does not belong to Customer with ID {customerId}");

            return account;
        }

        public async Task<bool>DeleteLoanAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);

            if(loan==null)
                return false;
            
            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}