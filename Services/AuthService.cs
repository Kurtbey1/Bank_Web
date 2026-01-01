using Bank_Project.Data;
using Bank_Project.DTOs;
using Bank_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bank_Project.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthService> _logger;
        private readonly PasswordHasher<Accounts> _passwordHasher;

        public AuthService(AppDbContext context, JwtService jwtService, ILogger<AuthService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
            _passwordHasher = new PasswordHasher<Accounts>();
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Login attempt failed: Request body is null.");
                throw new ArgumentNullException(nameof(dto));
            }

            _logger.LogInformation("Login attempt for user: {Email}", dto.Email);

            var account = await _context.Accounts
                .Include(a => a.Customers)
                .FirstOrDefaultAsync(a => a.Customers.Email == dto.Email);

            if (account == null || account.Customers == null)
            {
                _logger.LogWarning("Login denied: User {Email} not found", dto.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(account, account.PasswordHashed, dto.Password);

            if (result != PasswordVerificationResult.Success)
            {
                _logger.LogWarning("Login failed: Incorrect password for {Email}", dto.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            _logger.LogInformation("Login successful for {Email}", dto.Email);
            return _jwtService.GenerateToken(account.Customers.CUID, "Customer");
        }
    }
}