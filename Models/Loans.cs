using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Bank_Project.Models
{
    public class Loans
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoanID { get; set; }
        [Required]
        public string LoanStatus { get; set; }= string.Empty;
        [Required]
        public string LoanType = string.Empty;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The Loan Amount Greater than Zero")]
        public int LoanAmount { get; set; } 
        [Required]
        [Range(1,int.MaxValue, ErrorMessage ="The Payment Amount Greater than Zero")]
        public int PaymentAmount { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The Payment Interest Rate than Zero")]
        public decimal InterestRate { get; set; }
        [Required]
        [DataType("Date")]
        public DateTime StartDate { get; set; }
        [DataType("Date")]
        public DateTime EndDate { get; set; }
        public int CUID { get; set; }
        [ForeignKey(nameof(CUID))]
        public required Customers Customer { get; set; }

        public int EmpID { get; set; }
        [ForeignKey(nameof(EmpID))]
        public required Employees Employees { get; set; }

        public ICollection<Grants> Grants { get; set; } = new List<Grants>();


    }
}
