using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bank_Project.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly byte[] _key;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration config, ILogger<JwtService> logger)
        {
            _config = config;
            _logger = logger;
            var secret = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing in configuration!");
            _key = Encoding.UTF8.GetBytes(secret);
        }

        public string GenerateToken(int cuid, string role)
        {
            _logger.LogInformation("Generating JWT token for CUID: {CUID} with role: {Role}", cuid, role);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, cuid.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var signingKey = new SymmetricSecurityKey(_key);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpireMinutes"] ?? "60")),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            _logger.LogInformation("JWT token generated successfully for CUID: {CUID}", cuid);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}