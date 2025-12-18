using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bank_Project.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(int cuid, string role)
        {
            var key = _config["Jwt:Key"] ?? throw new Exception("Jwt:Key is missing");
            var issuer = _config["Jwt:Issuer"] ?? throw new Exception("Jwt:Issuer is missing");
            var audience = _config["Jwt:Audience"] ?? throw new Exception("Jwt:Audience is missing");
            var expireStr = _config["Jwt:ExpireMinutes"] ?? throw new Exception("Jwt:ExpireMinutes is missing");

            if (!int.TryParse(expireStr, out var expireMinutes))
                throw new Exception("Jwt:ExpireMinutes must be a number");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, cuid.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var key = _config["Jwt:Key"] ?? throw new Exception("Jwt:Key is missing");

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public object? GetClaimsFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null) return null;

            return principal.Claims.Select(c => new { c.Type, c.Value });
        }
    }
}