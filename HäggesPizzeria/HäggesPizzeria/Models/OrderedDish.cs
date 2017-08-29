using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HäggesPizzeria.Models
{
    public class OrderedDish
    {
        public int OrderedDishId { get; set; }
        public int BashDishId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public Order Order { get; set; }
        public Guid Guid { get; set; }
        [NotMapped]
        public ICollection<int> Ingredients { get; set; }
        public ICollection<OrderedDishIngredient> OrderedDishIngredients { get; set; }
    }
}
