using Bank_Project.DTOs;
namespace Bank_Project.Validators
{
    public interface ILoanValidator
    {
        void Validate(CreateLoanDto dto);
    }
}