using System;
using System.Collections.Generic;

namespace HäggesPizzeria.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<OrderedDish> OrderedDishes { get; set; }
    }
}
