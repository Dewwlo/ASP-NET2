using System.Collections.Generic;

namespace HäggesPizzeria.Models
{
    public class BaseDish
    {
        public int BaseDishId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public bool IsActive { get; set; }
        public ICollection<BaseDishIngredient> BaseDishIngredients { get; set; }
    }
}
