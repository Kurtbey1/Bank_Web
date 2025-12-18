using Bank_Project.Data;
using Bank_Project.DTOs;
using Bank_Project.Models;
using System;
using System.Threading.Tasks;

namespace Bank_Project.Services
{
    public class SignUpService
    {
        private readonly BankCoordinatorService _coordinator;

        public SignUpService(BankCoordinatorService coordinator)
        {
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
        }

        public async Task<Customers> RegisterAsync(
            CreateCustomerDto customerDto,
            CreateAccountDto accountDto,
            CreateCardDto? cardDto = null)
        {
            if (customerDto == null) throw new ArgumentNullException(nameof(customerDto));
            if (accountDto == null) throw new ArgumentNullException(nameof(accountDto));

            // Directly call coordinator
            var customer = await _coordinator.AddCustomerWithAccountAsync(customerDto, accountDto, cardDto);


            return customer;
        }
    }
}