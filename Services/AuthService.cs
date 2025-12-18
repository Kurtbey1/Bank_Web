using Bank_Project.Data;
using Bank_Project.DTOs;
using Bank_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Bank_Project.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // 1️⃣ Find the customer by email
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == dto.Email);

            if (customer == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            // 2️⃣ Find the account using the correct FK (CUID)
            var account = await _context.Accounts
                .Include(a => a.Cards) // optional, include cards if needed
                .FirstOrDefaultAsync(a => a.CUID == customer.CUID);

            if (account == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            // 3️⃣ Verify the password
            var passwordHasher = new PasswordHasher<Accounts>();
            var result = passwordHasher.VerifyHashedPassword(account, account.PasswordHashed, dto.Password);

            if (result != PasswordVerificationResult.Success)
                throw new UnauthorizedAccessException("Invalid email or password");

            // 4️⃣ Generate and return JWT token
            return _jwtService.GenerateToken(customer.CUID, "Customer");
        }
    }
}
