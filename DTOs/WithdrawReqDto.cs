using System.ComponentModel.DataAnnotations;

namespace Bank_Project.DTOs
{
    public class WithdrawReqDto
    {
        [Required]
        public int customerId { get; set; }
        [Required]

        public int accountId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "the amount must be more than 0 ")]
        public int amount { get; set; }
    }
}
