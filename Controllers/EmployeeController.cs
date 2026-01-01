using Bank_Project.DTOs;
using Bank_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Bank_Project.Controllers
{
    [ApiController]
    [Route("api/Employee")]
    
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeServices _EmpService;

        public EmployeeController(IEmployeeServices empService)
        {
            _EmpService = empService;
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("Loan")]
        public async Task<ActionResult> GiveLoansAsync(CreateLoanDto loanDto)
        {
            var Loan = await _EmpService.GiveLoanAsync(loanDto);
            return StatusCode(201, Loan);
        }


        [Authorize(Roles = "Employee,Admin")]
        [HttpGet("customers/{customerId}/accounts")]
        public async Task<ActionResult> CheckAccountCustomersAsync(int customerId)
        {
          
            var accounts = await _EmpService.GetCustomerAccountsAsync( customerId);

            if (accounts == null || !accounts.Any())
            {
                return NotFound(new { Message = $"No accounts found for Customer ID: {customerId}." });
            }   

            return Ok(accounts);
        }
    }

    

}
