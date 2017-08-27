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

        public int CalculateDishPrice(ICollection<Ingredient> ingredients, string dishName)
        {
            var temp = _context.BaseDishes.FirstOrDefault(bd => bd.Name == dishName);
            var basedishIngredients = _context.BaseDishIngredients.Where(bdi => bdi.BaseDishId == temp.BaseDishId).Select(i => i.Ingredient).ToList();
            //var addedIngredients = basedishIngredients.Where(bdi => !ingredients.Contains(bdi)).ToList();
            var sum = basedishIngredients.Sum(ai => ai.AddExtraPrice) + temp.Price;
            return sum;
        }
    }
}
