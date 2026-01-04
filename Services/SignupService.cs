using Bank_Project.DTOs;
using Bank_Project.Models;

namespace Bank_Project.Services
{
    public class SignUpService
    {
        private readonly BankCoordinatorService _coordinator;
        private readonly ILogger<SignUpService> _logger;    
        public SignUpService(BankCoordinatorService coordinator, ILogger<SignUpService> logger)
        {
            _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Customers> RegisterAsync(
            CreateCustomerDto? customerDto,
            CreateAccountDto? accountDto,
            CreateCardDto? cardDto=null)
        {
            _logger.LogInformation("Registering new customer...");
          
            if (customerDto == null || accountDto == null)
            {
                _logger.LogWarning("Registration failed: One or more DTOs are null.");
        
                ArgumentNullException.ThrowIfNull(customerDto);
                ArgumentNullException.ThrowIfNull(accountDto);
            }
            var customer = await _coordinator.AddCustomerWithAccountAsync(customerDto, accountDto, cardDto!);

            _logger.LogInformation("Customer registered successfully with CUED: {CUED}", customer.CUID);
            return customer;
        }
    }
}