using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Bank_Project.Models
{
    public class Employees
    {
        [Key]
        public int EmpID { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string SecondName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required]
        [Range(1,int.MaxValue,ErrorMessage ="the salary must be greater than 0")]
        public int Salary{ get; set; }

        [Required]
        [RegularExpression(@"^07\d{8}$", ErrorMessage = "Phone number must start with 07 and be 10 digits long.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;

        [Required]
        
        public bool IsDeleted { get; set; } = false;


        public int BranchID { get; set; }

        [ForeignKey(nameof(BranchID))]

        public required Branches Branches { get; set; }

        public int? SupervisorID { get; set; }  
        [ForeignKey(nameof(SupervisorID))]
        public Employees? Supervisor { get; set; } // Navigation Property
        public ICollection<Employees> Subordinate { get; set; } = new List<Employees>();
        public ICollection<Grants> Grants { get; set; } = new List<Grants>();





    }
}
