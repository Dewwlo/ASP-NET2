using System.Collections.Generic;

namespace HaggesPizzeria.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public ICollection<BaseDish> BaseDish { get; set; }
        public ICollection<OrderedDish> OrderedDish { get; set; }
    }
}
