using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
