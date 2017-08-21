using System.Collections.Generic;

namespace HäggesPizzeria.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public List<DishIngredient> DishIngredients { get; set; }
    }
}
