using Bank_Project.DTOs;
using Bank_Project.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bank_Project.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly SignUpService _signUpService;


        public AccountController(AuthService authService, SignUpService signUpService)
        {
            _authService = authService;
            _signUpService = signUpService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Validation Failed", Errors = errors });
            }
            try
            {
                var token = await _authService.LoginAsync(dto);


                return Ok(new { Token = token });
            }
            
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Server error: " + ex.Message });
            }

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Validation Failed", Errors = errors });
            }
            try
            {
                var customer = await _signUpService.RegisterAsync(dto.Customer, dto.Account, dto.Card);
                return Ok(new { Message = "Account created successfully", CustomerId = customer.CUID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Details = ex.Message });
            }
        }
    }
}