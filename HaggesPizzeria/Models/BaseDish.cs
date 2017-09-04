using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HaggesPizzeria.Models
{
    public class BaseDish
    {
        public int BaseDishId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Dish name")]
        public string Name { get; set; }
        [Required]
        public int Price { get; set; }
        public bool IsActive { get; set; }
        public Category Category { get; set; }
        public ICollection<BaseDishIngredient> BaseDishIngredients { get; set; }
    }
}
