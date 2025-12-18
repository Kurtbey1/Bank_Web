using System.ComponentModel.DataAnnotations;

namespace Bank_Project.DTOs
{
    public class LoginDto
    {
        public string Password { get; set; }= string.Empty;
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", ErrorMessage = "Enter with good layout : iskanBank1999@Example.com")]

        public string Email { get; set; } = string.Empty;
    }
}
