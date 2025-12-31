using Bank_Project.DTOs;
using Bank_Project.Models;

namespace Bank_Project.Services
{
    public interface IEmployeeServices
    {
        
        Task<LoanResultDto> GiveLoanAsync(CreateLoanDto dto);
        Task<string> SoftDeleteAsync(int customerId);
        Task<Loans>UpdateLoanDetailsAsync(int loanId,CreateLoanDto dto);
        Task<IEnumerable<Loans>> GetCustomersAllLoansAsync(int customerId);
        Task<IEnumerable<Accounts>>GetCustomerAccountsAsync(int customerId); 
        Task ReassignTheManagerAsync(int OldManagerId, int NewManagerId);
        Task<bool> DeleteLoanAsync(int loanId);



    }
}
