using System.Collections.Generic;

namespace HäggesPizzeria.Models
{
    public class DishIngredient
    {
        public int DishId { get; set; }
        public Dish Dish { get; set; }
        public int IngredintId { get; set; }
        public Ingredient Ingredient { get; set; }
    }
}
