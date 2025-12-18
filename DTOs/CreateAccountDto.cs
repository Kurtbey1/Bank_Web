namespace Bank_Project
{

    public class CreateAccountDto
    {
        public string AccountType { get; set; } = string.Empty;
        public int Balance { get; set; }
        public string CardType { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public int BranchId { get; set; }
    }




}