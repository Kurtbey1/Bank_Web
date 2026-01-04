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
        private readonly IEmployeeServices _empService;

        public EmployeeController(IEmployeeServices empService)
        {
            _empService = empService ?? throw new ArgumentNullException(nameof(empService));
            
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("Loan")]
        public async Task<ActionResult> GiveLoansAsync(CreateLoanDto loanDto)
        { 
            var loan = await _empService.GiveLoanAsync(loanDto);
            return StatusCode(201, loan);
        }


        [Authorize(Roles = "Employee,Admin")]
        [HttpGet("customers/{customerId}/accounts")]
        public async Task<ActionResult> CheckAccountCustomersAsync(int customerId)
        {
          
            var accounts = await _empService.GetCustomerAccountsAsync( customerId);

            if (!accounts.Any())
            {
                return NotFound(new { Message = $"No accounts found for Customer ID: {customerId}." });
            }  

            return Ok(accounts);
        }
        
        [Authorize(Roles = "Employee,Admin")]
        [HttpDelete("Employees/{empId}")]
        public async Task<IActionResult> DeleteEmployeeAsync(int empId)
        {
            var result = await _empService.SoftDeleteAsync(empId);

            if (result.StartsWith("Error"))
            {
                return BadRequest(new { Message = result });
            }
            return Ok(new {Message = result});
        }
        
        
        
        
        
    }

    

}
