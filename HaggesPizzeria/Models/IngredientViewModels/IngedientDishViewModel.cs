using System.Collections.Generic;

namespace HaggesPizzeria.Models.IngredientViewModels
{
    public class IngedientDishViewModel
    {
        public bool IsOrderedDish { get; set; }
        public int BaseDishId { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }
    }
}
