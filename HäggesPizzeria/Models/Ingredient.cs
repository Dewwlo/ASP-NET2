using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HäggesPizzeria.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Ingredient name")]
        public string Name { get; set; }
        [Required]
        public int AddExtraPrice { get; set; }
        public bool IsActive { get; set; }
        public ICollection<BaseDishIngredient> BaseDishIngredients { get; set; }
        public ICollection<OrderedDishIngredient> OrderedDishIngredients { get; set; }
    }
}
