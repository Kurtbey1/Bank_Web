namespace Bank_Project
{
    
public class CreateAccountDto
{
    public int CustomerId { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public int Balance { get; set; }
     public string CardType { get; set; } = string.Empty;
        public string CardPassword { get; set; } = string.Empty;
}



}