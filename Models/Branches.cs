using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Bank_Project.Models
{
    public class Branches
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BranchID { get; set; }
        [Required]
        public string BranchName { get; set; } = string.Empty;
        [Required]
        public string BranchAddress { get; set; } = string.Empty;
        public ICollection<Accounts> Accounts { get; set; } = new List<Accounts>();
        public ICollection<Employees>? Employees { get; set; }

    }
}
