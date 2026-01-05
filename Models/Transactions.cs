using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Bank_Project.Models
{
    [Index(nameof(AccountID))]
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactID { get; set; }

        [Required]
        public int AccountID { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public string TransactionType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Accounts Account { get; set; } = null!;
    }
    
}
        