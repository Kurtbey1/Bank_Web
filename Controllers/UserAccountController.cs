<<<<<<< HEAD
﻿using Bank_Project.DTOs;
using Bank_Project.Services;
using Microsoft.AspNetCore.Mvc;
=======
﻿using Bank_Project.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


>>>>>>> d77fbb882f905b8bc349125d0027ae447850e330
namespace Bank_Project.Controllers
{
    [ApiController]
    [Route("api/user-accounts")]
    //[Authorize]
    public class UserAccountController : ControllerBase
    {
<<<<<<< HEAD
        private readonly IAccountService _accountService;

        public UserAccountController(IAccountService accountService)
=======
        private readonly AccountService _accountService;

        public UserAccountController(AccountService accountService)
>>>>>>> d77fbb882f905b8bc349125d0027ae447850e330
        {
            _accountService = accountService;
        }

<<<<<<< HEAD
        [HttpPost("Deposit")]
        public async Task<ActionResult> Deposit([FromBody] DepositReqDto dto)
=======
        [HttpGet("primary/{customerId}")]
        public async Task<IActionResult> GetPrimaryAccount([FromRoute] int customerId)
>>>>>>> d77fbb882f905b8bc349125d0027ae447850e330
        {
            await _accountService.DepositAsync(
                dto.customerId,
                dto.accountId,
                dto.amount
                
            );

<<<<<<< HEAD
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
=======
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
>>>>>>> d77fbb882f905b8bc349125d0027ae447850e330
