using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Bank_Project.Models
{
    [Index(nameof(AccountID))]
    public class Accounts
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int AccountID { get; set; }

        [Required]
        public string AccountType { get; set; }=string.Empty;
        [Required]
        [NonNegativeNumber]
        public int Balance { get; set; }
        [Required]
        public int BranchID { get; set; }
        [Required]
        public string PasswordHashed { get; set; } = string.Empty;
        [Required]
        [Column("CUID")]
        public int CUID{ get; set; }
        [Required]
        [Column(TypeName = "date")]
        public DateTime openDate { get; set; }
        [Required]
        [ForeignKey(nameof(BranchID))]
        public required Branches Branches { get; set; }

        [ForeignKey("CUID")]
        public virtual Customers Customer { get; set; } = null!;
        public Cards? Cards { get; set; }
        public virtual ICollection<Transactions> Transactions { get; set; } = new List<Transactions>();





    } 
}
public class NonNegativeNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int number && number >= 0)
            return ValidationResult.Success!;

        return new ValidationResult("Balance must be 0 or greater");
    }
}