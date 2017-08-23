using System.Collections.Generic;
using System.Linq;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;

namespace HäggesPizzeria.Services
{
    public class IngredientService
    {
        private readonly ApplicationDbContext _context;

        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<Ingredient> GetAllActiveIngredients(ICollection<BaseDishIngredient> usedIngredients)
        {
            return _context.Ingredients.Where(i => usedIngredients.All(ui => ui.IngredientId != i.IngredientId) && i.IsActive).ToList();
        }
    }
}
