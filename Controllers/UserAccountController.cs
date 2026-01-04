using Bank_Project.DTOs;
using Bank_Project.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Project.Controllers
{
    [ApiController]
    [Route("api/user-accounts")]
    // [Authorize]
    public class UserAccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public UserAccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }   

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositReqDto dto)
        {
            await _accountService.DepositAsync(
                dto.customerId,
                dto.accountId,
                dto.amount
            );

            return Ok(new { Message = "Deposit successful" });
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawReqDto dto)
        {


            await _accountService.WithdrawAsync(
                dto.customerId,
                dto.accountId,
                dto.amount
            );

            return Ok(new { Message = "Withdraw successful" });
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransfareReqDto dto)
        {
            

            await _accountService.TransferAsync(
                dto.senderId,
                dto.reciverId,
                dto.fromAccount,
                dto.toAccount,
                dto.amount
            );

            return Ok(new { Message = "Transfer successful" });
        }

        [HttpGet("customers/{customerId}/primary-account")]
        public async Task<IActionResult> GetPrimaryAccount([FromRoute] int customerId)
        {
            var account = await _accountService.GetPrimaryAccountAsync(customerId);

            if (account == null)
                return NotFound(new { Message = $"No primary account found for customer {customerId}" });

            return Ok(account);
        }
    }
}