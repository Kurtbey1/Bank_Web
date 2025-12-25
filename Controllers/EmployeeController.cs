using Bank_Project.DTOs;
using Bank_Project.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bank_Project.Controllers
{
    [ApiController]
    [Route("Employee")]
    public class EmployeeController : ControllerBase
    {
        private EmployeeServices _EmpService;

        public EmployeeController(EmployeeServices empService)
        {
            _EmpService = empService;
        }

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
                return Ok(Loan);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Sarver Error : "+ex.Message});
            }
        }

        
    }
}
