namespace Bank_Project.DTOs
{
    public class CreateLoanDto
    {

        
        public int CustomerId { get; set; }
        public int LoanId { get; set; }
        public string LoanStatus { get; set; } = string.Empty;
        public string LoanType { get; set; } = string.Empty;
        public int LoanAmount { get; set; } 
        public int PaymentAmount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 

        public  int EmpID{get;set;}

    }









}