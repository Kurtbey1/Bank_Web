using Bank_Project.DTOs;
using Bank_Project.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly SignUpService _signUpService;

    public AuthController(AuthService authService, SignUpService signUpService)
    {
        _authService = authService;
        _signUpService = signUpService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(new { Token = token });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "An internal error occurred." });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var customer = await _signUpService.RegisterAsync(dto.Customer, dto.Account, dto.Card);
            return Ok(new { Message = "Account created successfully", CustomerId = customer.CUID });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Registration failed", Details = ex.Message });
        }
    }
}