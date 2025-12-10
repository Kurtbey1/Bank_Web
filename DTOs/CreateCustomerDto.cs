namespace Bank_Project.DTOs
{
    public class CreateCustomerDto
    {
        public string FirstName { get; set; }=string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Salary { get; set; }

        public int BranchID { get; set; }

        public string CardNumber { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
        public string CardPassword { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;


        public string AccountType { get; set; }= string.Empty;

        public int Balance { get; set; } 

    }
}
