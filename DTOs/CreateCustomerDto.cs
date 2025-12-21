using System.ComponentModel.DataAnnotations;

namespace Bank_Project.DTOs
{
    public class CreateCustomerDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^07\d{8}$", ErrorMessage = "Phone number must start with 07 and be 10 digits long.")]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Enter with good layout: example@domain.com")]

        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Salary { get; set; }
    }

}
