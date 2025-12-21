using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Bank_Project.Models
{
    public class Customers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int CUID { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string SecondName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "date")]
        public DateTime BirthDate { get; set; }

        [Required]
        [RegularExpression(@"^07\d{8}$", ErrorMessage = "Phone number must start with 07 and be 10 digits long.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Salary must be greater than 0")]
        public int Salary { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Accounts> Accounts { get; set; } = new List<Accounts>();
        public ICollection<Cards> Cards { get; set; } = new List<Cards>();
        public ICollection<Loans> Loans { get; set; } = new List<Loans>();
    }
}