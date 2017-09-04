using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HäggesPizzeria.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        [Required]
        [Display(Name = "Total order price")]
        public int TotalPrice { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Delivery adress")]
        public string Adress { get; set; }
        [Required]
        [StringLength(5, ErrorMessage = "The {0} must be {2} characters long.", MinimumLength = 5)]
        public string Zipcode { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public bool IsComplete { get; set; }
        public DateTime OrderDate { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<OrderedDish> OrderedDishes { get; set; }
    }
}
