using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bank_Project.Models
{
    public class Grants
    {

        public int LoanID { get; set; }
        [ForeignKey(nameof(LoanID))]
        public required Loans Loan { get; set; }

        public int EmpID { get; set; }
        [ForeignKey(nameof(EmpID))]
        public required Employees Employee { get; set; }

    }
}