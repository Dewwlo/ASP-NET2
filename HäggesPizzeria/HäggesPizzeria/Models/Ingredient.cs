using System.Collections.Generic;

namespace HäggesPizzeria.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public int AddExtraPrice { get; set; }
        public ICollection<BaseDishIngredient> BaseDishIngredients { get; set; }
        public ICollection<OrderedDishIngredient> OrderedDishIngredients { get; set; }
    }
}
