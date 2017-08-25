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

        public ICollection<Ingredient> GetAllUnusedIngredients(ICollection<Ingredient> usedIngredients)
        {
            return _context.Ingredients.Where(i => usedIngredients.All(ui => ui.IngredientId != i.IngredientId) && i.IsActive).ToList();
        }

        public ICollection<Ingredient> GetAllUsedIngredients(ICollection<BaseDishIngredient> usedIngredients)
        {
            return _context.Ingredients.Where(i => usedIngredients.Any(ui => ui.Ingredient == i)).ToList();
        }

        public ICollection<Ingredient> GetAllUsedIngredients(ICollection<int> usedIngredients)
        {
            return _context.Ingredients.Where(i => usedIngredients.Any(ui => ui == i.IngredientId)).ToList();
        }
    }
}
