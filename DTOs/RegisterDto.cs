using Bank_Project;
using Bank_Project.DTOs;

public class RegisterDto
{
    public required CreateCustomerDto Customer { get; set; }
    public required CreateAccountDto Account { get; set; }
    public required CreateCardDto Card { get; set; }
}