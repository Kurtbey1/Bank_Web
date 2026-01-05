using Bank_Project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Customers
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CUID { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    public DateTime BirthDate { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Email { get; set; } = string.Empty;

    // ✅ العلاقات الصحيحة فقط
    public ICollection<Accounts> Accounts { get; set; } = new List<Accounts>();
    public ICollection<Loans> Loans { get; set; } = new List<Loans>();
}