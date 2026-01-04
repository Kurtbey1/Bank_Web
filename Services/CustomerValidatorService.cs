using Bank_Project.DTOs;

namespace Bank_Project.Services
{
    public class CustomerValidatorService : ICustomerValidatorService
    {
        public void ValidateCustomerDto(CreateCustomerDto dto)
        {
            if (dto == null)
            {
                
                throw new ArgumentNullException(nameof(dto));
            }
            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                throw new ArgumentException("First name required");
            }
            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                throw new ArgumentException("Last name required");
            }
            if (dto.Salary <= 0)
            {
                throw new ArgumentException("Salary must be greater than 0");

            }
        }
    }

}

