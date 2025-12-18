using Bank_Project.DTOs;

namespace Bank_Project.Services
{
    public interface ICustomerValidatorService
    {
        void ValidateCustomerDto(CreateCustomerDto dto);
    }
}
