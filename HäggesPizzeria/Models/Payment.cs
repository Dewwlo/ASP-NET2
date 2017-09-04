using System.ComponentModel.DataAnnotations;

namespace HäggesPizzeria.Models
{
    public class Payment
    {
        [Required]
        [MaxLength(16)]
        // TODO Find out why regex does not work.
        //[RegularExpression("[^[0-9]*$]")]
        [Display(Name = "Credit card number")]
        public string CardNumber { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        [MaxLength(3)]
        //[RegularExpression("[^[0-9]*$]")]
        public string Cvc { get; set; }
    }
}
