 using Bank_Project.Data;
using Bank_Project.DTOs;
using Bank_Project.Models;
using Bank_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bank_Project.Controllers
{
    [ApiController]
    [Route("api/UserAccount")]
    //[Authorize]
    public class UserAccountController : ControllerBase
    {
        private readonly BankCoordinatorService _coordinator;
        private readonly AccountService _accountService;

        public UserAccountController(BankCoordinatorService coordinator, AccountService accountService)
        {
            _coordinator = coordinator;
            _accountService = accountService;
        }

        [HttpGet("{CustomerId}")]
        public async Task<IActionResult> GetPrimaryAccount([FromRoute] int CustomerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Validation Failed", Errors = errors });
            }

            try { 
            var customer = await _accountService.GetPrimaryAccountAsync(CustomerId);

                if (customer == null)
                {
                    return BadRequest(new { Message = $"No primary account found for Customer ID: {CustomerId}" });
                }

                return Ok(customer);
            }
            

            catch (Exception ex){
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }



    }
}
