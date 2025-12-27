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

        public JwtService(IConfiguration config)
        {
            _config = config;
            var secret = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing in configuration!");
            _key = Encoding.UTF8.GetBytes(secret);
        }

        public string GenerateToken(int cuid, string role)
        {
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

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}