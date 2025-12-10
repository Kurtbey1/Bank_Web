
using Bank_Project.Models;

namespace Bank_Project.Services
{
    public interface IEmployeeServices
    {
        Task<Loans> GiveLoanAsync(int customerId, Loans loans);

        Task<Loans?>CheckAccountCustomersAsync(int customerId); 


    }
}
