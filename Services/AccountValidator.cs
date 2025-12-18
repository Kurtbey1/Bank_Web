using Bank_Project.DTOs;

namespace Bank_Project.Services
{
    public class AccountValidator
    {
        public static void ValidateAccountDto(CreateAccountDto dto,CreateCardDto Cdto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Balance < 0) throw new ArgumentException("Balance cannot be negative");
            if (string.IsNullOrWhiteSpace(dto.AccountType)) throw new ArgumentException("Account type required");
            if (string.IsNullOrWhiteSpace(dto.CardType)) throw new ArgumentException("Card type required");
            if (string.IsNullOrWhiteSpace(Cdto.CardPassword)) throw new ArgumentException("Card password required");
        }
    }
}
