using Bank_Project.DTOs;
using Bank_Project.Data;
using Bank_Project.Models;
using Bank_Project.Validators;
using Microsoft.EntityFrameworkCore;


namespace Bank_Project.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly AppDbContext _context;
        private readonly ILoanValidator _loanValidator;

        public EmployeeServices(AppDbContext context, ILoanValidator loanValidator)
        {
            _context = context;
            _loanValidator = loanValidator;
        }

        public async Task<Loans> GiveLoanAsync(CreateLoanDto dto)
        {
            _loanValidator.Validate(dto);

            var customer = await _context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.CUID == dto.CustomerId)
                ?? throw new Exception("Customer not found");

            var emp = await _context.Employees.FindAsync(dto.EmpID)
                ?? throw new Exception("Employee not found");

            var account = customer.Accounts?.FirstOrDefault()
                ?? throw new Exception("Customer has no account");

            var loan = new Loans
            {
                CUID = dto.CustomerId,
                Customer = customer,
                Employees = emp,
                EmpID = dto.EmpID,
                LoanStatus = dto.LoanStatus,
                LoanType = dto.LoanType,
                LoanAmount = dto.LoanAmount,
                PaymentAmount = dto.PaymentAmount,
                InterestRate = dto.InterestRate,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            account.Balance += dto.LoanAmount;

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<Loans> UpdateLoanDetailsAsync(int loanId, CreateLoanDto dto)
        {
            _loanValidator.Validate(dto);

            var loan = await _context.Loans
                .Include(l => l.Customer)
                .ThenInclude(c => c.Accounts)
                .FirstOrDefaultAsync(l => l.LoanID == loanId)
                ?? throw new KeyNotFoundException("Loan not found");

            var emp = await _context.Employees.FindAsync(dto.EmpID)
                ?? throw new KeyNotFoundException("Employee not found");

            var account = loan.Customer.Accounts?.FirstOrDefault()
                ?? throw new Exception("Customer has no account");

            account.Balance += dto.LoanAmount - loan.LoanAmount;

            loan.LoanStatus = dto.LoanStatus;
            loan.LoanType = dto.LoanType;
            loan.LoanAmount = dto.LoanAmount;
            loan.PaymentAmount = dto.PaymentAmount;
            loan.InterestRate = dto.InterestRate;
            loan.StartDate = dto.StartDate;
            loan.EndDate = dto.EndDate;
            loan.EmpID = dto.EmpID;
            loan.Employees = emp;

            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task<IEnumerable<Loans>> GetCustomersAllLoansAsync(int customerId)
        {
            var exists = await _context.Customers
                .AnyAsync(c => c.CUID == customerId);

            if (!exists)
                throw new Exception("Customer not found");

            return await _context.Loans
                .Where(l => l.CUID == customerId)
                .ToListAsync();
        }
        


        public async Task<Accounts?> CheckAccountCustomersAsync(int customerId, int accountId)
        {
            return await _context.Accounts
                .SingleOrDefaultAsync(a =>
                    a.AccountID == accountId &&
                    a.Customers.CUID == customerId);
        }

        public async Task<bool> DeleteLoanAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null)
                return false;

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}