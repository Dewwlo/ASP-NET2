using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HäggesPizzeria.Models.IngredientViewModels
{
    public class IngedientDishViewModel
    {
        public bool IsOrderedDish { get; set; }
        public int BaseDishId { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }
    }
}
