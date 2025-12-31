using System.ComponentModel.DataAnnotations;

namespace Bank_Project
{

    public class CreateAccountDto
    {
        public string AccountType { get; set; } = string.Empty;
        public int Balance { get; set; }
        public string CardType { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
    ErrorMessage = "Password too weak: Must contain at least one uppercase letter, one lowercase letter, one number, one special character, and be at least 8 characters long.")]
        public string PasswordHashed { get; set; } = string.Empty;
        public int BranchId { get; set; }
    }




}