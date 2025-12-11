using Bank_Project.DTOs;
using Bank_Project.Models;

namespace Bank_Project.Services
{
    public interface IEmployeeServices
    {
        
        Task<Loans> GiveLoanAsync(CreateLoanDto dto);
        Task<Loans>UpdateLoanDetailsAsync(int loanId,CreateLoanDto dto);
        Task<IEnumerable<Loans>> GetCustomersAllLoansAsync(int customerId);
        Task<Accounts?>CheckAccountCustomersAsync(int customerId,int loanId); 
        Task<bool> DeleteLoanAsync(int loanId);



    }
}
