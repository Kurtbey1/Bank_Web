using Bank_Project.DTOs;
using Bank_Project.Services;
using Microsoft.AspNetCore.Mvc;
namespace Bank_Project.Controllers
{
    [ApiController]
    [Route("api/UserAccount")]
    //[Authorize]
    public class UserAccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public UserAccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Deposit")]
        public async Task<ActionResult> Deposit([FromBody] DepositReqDto dto)
        {
            await _accountService.DepositAsync(
                dto.customerId,
                dto.accountId,
                dto.amount
                
            );

            return Ok(new { Message = "Deposit Successful" });
        }


        [HttpPost("Withdraw")]
        public async Task<ActionResult> Withdraw([FromBody] DepositReqDto dto)
        {
            await _accountService.WithdrawAsync(
               dto.customerId,
               dto.accountId,
               dto.amount

           );

            return Ok(new { Message = "Withdraw Successful" });
        }



        [HttpPost("Transfer")]
        public async Task<ActionResult> Transfer([FromBody] TransfareReqDto dto)
        {
            await _accountService.TransferAsync(
               dto.senderId,
               dto.reciverId,
               dto.fromAccount,
               dto.toAccount,
               dto.amount

           );

            return Ok(new { Message = "Transfer Money Successful" });
        }





        [HttpGet("customers/{CustomerId}")]
        public async Task<IActionResult> GetPrimaryAccount([FromRoute] int CustomerId)
        {
         
            
           var customer = await _accountService.GetPrimaryAccountAsync(CustomerId);

           if (customer == null)
           {
               return NotFound(new { Message = $"No primary account found for Customer ID: {CustomerId}" });
           }

           return Ok(customer);
               
        }
        
    }
}
