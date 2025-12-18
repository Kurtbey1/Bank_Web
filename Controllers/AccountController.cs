using Microsoft.AspNetCore.Mvc;
using Bank_Project.DTOs;
using Bank_Project.Services;
using System;
using System.Threading.Tasks;

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
                return BadRequest(ModelState);

            try
            {
                var token = await _authService.LoginAsync(dto);

                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                });

                return Ok(new { Token = token });
            }
            //catch (UnauthorizedAccessException)
            //{
            //    return Unauthorized(new { Message = "Invalid email or password" });
            //}
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
        return BadRequest(ModelState);

    try
    {
        var customer = await _signUpService.RegisterAsync(dto.Customer, dto.Account, dto.Card);
        return Ok(new { Message = "Account created successfully", CustomerId = customer.CUID });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { Message = ex.Message });
    }
}
    }
}