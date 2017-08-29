using System.Collections.Generic;
using System.Linq;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

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

        public int CalculateDishPrice(ICollection<Ingredient> ingredients, int baseDishId)
        {
            var baseDish = _context.BaseDishes.SingleOrDefault(bd => bd.BaseDishId == baseDishId);
            var basedishIngredients = _context.BaseDishIngredients.Where(bdi => bdi.BaseDishId == baseDish.BaseDishId).Select(i => i.Ingredient).ToList();
            var addedIngredients = ingredients.Where(i => !basedishIngredients.Select(bdi => bdi.IngredientId).Contains(i.IngredientId)).ToList();
            return addedIngredients.Sum(ai => ai.AddExtraPrice) + baseDish.Price;
        }
    }
}
