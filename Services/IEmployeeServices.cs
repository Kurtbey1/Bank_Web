using Bank_Project.DTOs;
using Bank_Project.Models;

namespace Bank_Project.Services
{
    public interface IEmployeeServices
    {
        
        Task<Loans?>CheckAccountCustomersAsync(int customerId); 

        Task<Loans> GiveLoanAsync(CreateLoanDto dto);
        
        Task<IEnumerable<Loans>> GetCustomersAllLoansAsync(int customerId);

        Task<Loans>UpdateLoanDetailsAsync(int loanId,Loans loans);

        Task<bool> DeleteLoanAsync(int loanId);



    }
}
