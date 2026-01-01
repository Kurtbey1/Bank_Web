using Bank_Project.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Project.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDto dto);
    }
}
