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
        private readonly ILogger<EmployeeServices> _logger;

        public EmployeeServices(AppDbContext context, ILoanValidator loanValidator, ILogger<EmployeeServices> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _loanValidator = loanValidator?? throw new ArgumentNullException(nameof(loanValidator));
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoanResultDto> GiveLoanAsync(CreateLoanDto dto)
        {
            _logger.LogInformation("Creating Loan");

            _loanValidator.Validate(dto);

            var customer = await _context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.CUID == dto.CustomerId)
                ?? throw new Exception("Customer not found");

            var emp = await _context.Employees.FindAsync(dto.EmpID)
                ?? throw new Exception("Employee not found");

            var account = customer.Accounts.FirstOrDefault()
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
            _logger.LogInformation("Loan Created Successfully for customer with id {OldID} by customer with ID{NewID}",dto.CustomerId,dto.EmpID);
            return new LoanResultDto
            {
                CustomerId = customer.CUID,
                AmountGranted = loan.LoanAmount, 
                NewBalance = account.Balance,
                intrestRate = loan.InterestRate,
            };

        }

        public async Task<Loans> UpdateLoanDetailsAsync(int loanId, CreateLoanDto dto)
        {
            _logger.LogInformation("Updating Loan");

            _loanValidator.Validate(dto);

            var loan = await _context.Loans
                .Include(l => l.Customer)
                .ThenInclude(c => c.Accounts)
                .FirstOrDefaultAsync(l => l.LoanID == loanId)
                ?? throw new KeyNotFoundException("Loan not found");

            var emp = await _context.Employees.FindAsync(dto.EmpID)
                ?? throw new KeyNotFoundException("Employee not found");

            var account = loan.Customer.Accounts.FirstOrDefault()
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
            _logger.LogInformation("The Loan Updated Successfully");
            return loan;
        }

        public async Task<IEnumerable<Loans>> GetCustomersAllLoansAsync(int customerId)
        {
            _logger.LogInformation("Getting All Loans For Customer With Id {ID}",customerId);
            var exists = await _context.Customers
                .AnyAsync(c => c.CUID == customerId);

            if (!exists)
            {
                _logger.LogWarning("Collecting Data Failed: Customer With Id {id} Isn't Found",customerId);
                throw new Exception("Customer not found");
            }
            _logger.LogInformation("Process Run Successfully");
            return await _context.Loans
                .Where(l => l.CUID == customerId)
                .ToListAsync();
        }
        


    public async Task<IEnumerable<Accounts>> GetCustomerAccountsAsync(int customerId)
    {

        var accounts = await _context.Accounts
            .Where(a => a.CUID == customerId)
            .ToListAsync();

        if (!accounts.Any())
        { 
         _logger.LogWarning("No accounts found for customer with ID {ID}", customerId);
        return Enumerable.Empty<Accounts>(); 
        }
        _logger.LogInformation("Process Run Successfully");

           return accounts;
        }

        public async Task<bool> DeleteLoanAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null)
            {
                _logger.LogWarning("Delete Failed: Loan With ID {ID} Is Not Found", loanId);
                return false;
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Loan Deleted Successfully");
            return true;
        }

        public async Task<string> SoftDeleteAsync(int empId)
        {
            _logger.LogInformation("Soft Deleting Employee with ID {ID}", empId);
            var employee = await _context.Employees
                .Include(e => e.Subordinate)
                .FirstOrDefaultAsync(e => e.EmpID == empId);
            if (employee == null)
                return "Error : The Employee Isn't Found";

            if (employee.Subordinate.Any(s => !s.IsDeleted))
                return "You Must Reassign The Subordinate To Another Manager";

            employee.IsDeleted = true;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Employee with ID {ID} soft deleted successfully", empId);
            return "The Employee Has Been Deleted";
        }

        public async Task ReassignTheManagerAsync(int oldManagerId, int newManagerId)
        {
            _logger.LogInformation("Reassigning subordinates from Manager {OldID} to Manager {NewID}", oldManagerId, newManagerId);
            var subordinate = await _context.Employees.Where(e => e.SupervisorID == oldManagerId && !e.IsDeleted).ToListAsync();

           foreach(var emp  in subordinate)
            {
                emp.SupervisorID = newManagerId;
            }
            _logger.LogInformation("Reassignment completed successfully");
           await _context.SaveChangesAsync();
        }
    }
}