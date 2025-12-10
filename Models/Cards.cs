using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Bank_Project.Models
{
    public class Cards
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CardID { get; set; }

        // Card Number - 16 Digit (Luhn Valid)
        [Required]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be 16 digits.")]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

      
        [Required]
        public string CardType { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be exactly 3 digits.")]
        public string CVV { get; set; } = string.Empty;


        [Required]
        public string PasswordHash { get; set; } = string.Empty;


        public int AccountID { get; set; }
        [ForeignKey(nameof(AccountID))]
        public required Accounts Account { get; set; }


        private static readonly Random rand = new Random();

  
        public static string GenerateCardNumber()
        {
            int[] digits = new int[16];

           for (int i = 0; i < 15; i++)
                digits[i] = rand.Next(0, 10);

 
            int sum = 0;
            for (int i = 0; i < 15; i++)
            {
                int num = digits[14 - i];
                if (i % 2 == 0)
                {
                    num *= 2;
                    if (num > 9) num -= 9;
                }
                sum += num;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            digits[15] = checkDigit;

            return string.Concat(digits);
        }

        public static string GenerateCVV()
        {
            return rand.Next(100, 1000).ToString();
        }
    }
}