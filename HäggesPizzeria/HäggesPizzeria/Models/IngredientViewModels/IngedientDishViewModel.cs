using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HäggesPizzeria.Models.IngredientViewModels
{
    public class IngedientDishViewModel
    {
        public bool IsOrderedDish { get; set; }
        public String DishName { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }
    }
}
