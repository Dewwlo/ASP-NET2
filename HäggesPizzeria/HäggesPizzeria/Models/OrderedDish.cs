using System.Collections.Generic;

namespace HäggesPizzeria.Models
{
    public class OrderedDish
    {
        public int OrderedDishId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public Order Order { get; set; }
        public ICollection<OrderedDishIngredient> OrderedDishIngredients { get; set; }
    }
}
