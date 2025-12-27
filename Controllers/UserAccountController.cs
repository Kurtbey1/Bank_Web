using Bank_Project.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace Bank_Project.Controllers
{
    [ApiController]
    [Route("api/user-accounts")]
    //[Authorize]
    public class UserAccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public UserAccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("primary/{customerId}")]
        public async Task<IActionResult> GetPrimaryAccount([FromRoute] int customerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Validation Failed", Errors = errors });
            }

            try { 
            var customer = await _accountService.GetPrimaryAccountAsync(customerId);

                if (customer == null)
                {
                    return NotFound(new { Message = $"No primary account found for Customer ID: {customerId}" });
                }

                return Ok(customer);
            }
            
            catch(SqlException)
            {
                return StatusCode(500, "Database Connection Failed.");
            }
            catch (NullReferenceException ex) 
            {
                return StatusCode(500, new { Message = "Code error: Null reference detected.", Detail = ex.Message });
            }
            catch (Exception  ex)
            {
                return StatusCode(500, new { Message = "Sarver Error : "+ex.Message});
   
            }



        }
    } 
}