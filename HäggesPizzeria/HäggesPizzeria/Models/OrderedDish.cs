using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HäggesPizzeria.Models
{
    public class OrderedDish
    {
        public int OrderedDishId { get; set; }
        public int BaseDishId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        public int Price { get; set; }
        public Category Category { get; set; }
        public Order Order { get; set; }
        public Guid Guid { get; set; }
        [NotMapped]
        public ICollection<int> Ingredients { get; set; }
        public ICollection<OrderedDishIngredient> OrderedDishIngredients { get; set; }
    }
}
