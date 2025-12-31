using Bank_Project.DTOs;
using Bank_Project.Models;
using Bank_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bank_Project.Controllers
{
    [ApiController]
    [Route("Employee")]
    
    public class EmployeeController : ControllerBase
    {
        private IEmployeeServices _EmpService;

        public EmployeeController(IEmployeeServices empService)
        {
            _EmpService = empService;
        }
        [Authorize(Roles = "Employee")]
        [HttpPost("Loan")]
        public async Task<ActionResult> GiveLoans([FromBody]CreateLoanDto loanDto)
        {
            if (!ModelState.IsValid)
            {
                var errors= ModelState.Values.SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage).ToList();
                return BadRequest(new {Message="validation Failed",Errors = errors});
            }

            try
            {
                var Loan = await _EmpService.GiveLoanAsync(loanDto);
                return StatusCode(201, Loan);
            }
            catch(SqlException )
            {
                return StatusCode(500, "Database Connection Failed.");
            }
            catch (NullReferenceException ex) 
            {
                return StatusCode(500, new { Message = "Code error: Null reference detected.", Detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Sarver Error : "+ex.Message});
            }

        }
        [Authorize(Roles = "Employee,Admin")]
        [HttpGet("customers/{customerId}/accounts")]
        public async Task<ActionResult> CheckAccountCustomersAsync([FromRoute] int customerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(V=>V.Errors).Select(e=>e.ErrorMessage).ToList();
                return BadRequest(new {Message="validation Failed",Errors = errors});
            }


            try
            {
                var accounts = await _EmpService.GetCustomerAccountsAsync( customerId);

                if (accounts == null || !accounts.Any())
                {
                    return NotFound(new { Message = $"No accounts found for Customer ID: {customerId}." });
                }   

                return Ok(accounts);
            }
            catch(Exception ex)
            {
                    return StatusCode(500,new { Message = $"Internal Server Error occurred while retrieving accounts."  ,Detail = ex.Message});

            }
        }
    }

    

}
