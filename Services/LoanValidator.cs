using Bank_Project.DTOs;

namespace Bank_Project.Validators
{
    public class LoanValidator : ILoanValidator
    {
        public void Validate(CreateLoanDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.LoanStatus))
                throw new ArgumentException("Loan status is required");

            if (string.IsNullOrWhiteSpace(dto.LoanType))
                throw new ArgumentException("Loan type is required");

            if (dto.LoanAmount <= 0)
                throw new ArgumentException("Loan amount must be greater than zero");

            if (dto.PaymentAmount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero");

            if (dto.InterestRate <= 0)
                throw new ArgumentException("Interest rate must be greater than zero");

            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("Invalid loan duration");

            if (dto.EmpID <= 0)
                throw new ArgumentException("Invalid employee id");
        }
    }
}
