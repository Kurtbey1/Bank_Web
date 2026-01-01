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
    public async Task<IActionResult> LoginAsync(LoginDto dto)
    {

        var token = await _authService.LoginAsync(dto);

        if (string.IsNullOrEmpty(token))
            return Unauthorized(new { Message = "Invalid credentials." });

        return Ok(new { Token = token });
        

    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto dto)
    {
        var customer = await _signUpService.RegisterAsync(dto.Customer, dto.Account, dto.Card);

        if (customer == null)
            return BadRequest(new { Message = "Registration failed. Email might already exist." });

        return Created(string.Empty, new
        {
            Message = "Account created successfully",
            CustomerId = customer.CUID
        });

    }
}