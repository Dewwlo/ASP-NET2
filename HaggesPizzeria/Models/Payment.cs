using System.ComponentModel.DataAnnotations;

namespace HaggesPizzeria.Models
{
    public class Payment
    {
        [Required]
        [MaxLength(16, ErrorMessage = "Not a valid Credit card number")]
        [MinLength(16, ErrorMessage = "Not a valid Credit card number")]
        [RegularExpression(@"\d+", ErrorMessage = "Not a valid credit card number")]
        [Display(Name = "Credit card number")]
        public string CardNumber { get; set; }
        [Required]
        [Range(2017, 2050)]
        public int Year { get; set; }
        [Required]
        [Range(1, 12)]
        public int Month { get; set; }
        [Required]
        [MaxLength(3, ErrorMessage = "Not a valid CVC number")]
        [MinLength(3, ErrorMessage = "Not a valid CVC number")]
        [RegularExpression(@"\d+", ErrorMessage = "Not a valid CVC number")]
        public string Cvc { get; set; }
    }
}
