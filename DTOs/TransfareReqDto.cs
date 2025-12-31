using System.ComponentModel.DataAnnotations;

namespace Bank_Project.DTOs
{
    public class TransfareReqDto
    {
        [Required(ErrorMessage = "senderId ID is required.")]
        public int senderId {  get; set; }

        [Required(ErrorMessage = "reciverId ID is required.")]
        public int reciverId{ get; set; }
        [Required(ErrorMessage = "sender's Account ID is required.")]

        public int fromAccount{ get; set; }
        [Required(ErrorMessage = "Recivere's Account ID is required.")]

        public int toAccount { get; set; }
        [Required(ErrorMessage = "The Amount Of Money is required.")]

        public int amount { get; set; }

    }
}
