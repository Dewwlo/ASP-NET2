using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HäggesPizzeria.Models
{
    public class Payment
    {
        [Required]
        [MaxLength(16)]
        //[RegularExpression("[^[0-9]{1,16}$]")]
        public string CardNumber { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        [MaxLength(3)]
        //[RegularExpression("[^[0-9]{1,16}$]")]
        public string Cvc { get; set; }
    }
}
