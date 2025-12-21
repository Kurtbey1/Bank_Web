namespace Bank_Project.DTOs
{
    public class AccountRespDto
    {
        public int AccountId {  get; set; }
        public string AccountType{ get; set; } = string.Empty;
         
        public int Balance { get; set; }
    }
}
