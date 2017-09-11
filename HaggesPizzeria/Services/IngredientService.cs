using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaggesPizzeria.Data;
using HaggesPizzeria.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HaggesPizzeria.Services
{
    public class IngredientService
    {
        private readonly ApplicationDbContext _context;
        private readonly BaseDishService _baseDishService;

        public IngredientService(ApplicationDbContext context, BaseDishService baseDishService)
        {
            _context = context;
            _baseDishService = baseDishService;
        }

        public ICollection<Ingredient> GetAllUnusedIngredients(ICollection<Ingredient> usedIngredients)
        {
            return _context.Ingredients.Where(i => usedIngredients.All(ui => ui.IngredientId != i.IngredientId) && i.IsActive).OrderBy(i => i.Name).ToList();
        }

        public ICollection<Ingredient> GetAllUsedIngredients(ICollection<BaseDishIngredient> usedIngredients)
        {
            return _context.Ingredients.Where(i => usedIngredients.Any(ui => ui.Ingredient == i)).OrderBy(i => i.Name).ToList();
        }

        public ICollection<Ingredient> GetAllUsedIngredients(ICollection<int> usedIngredients)
        {
            return _context.Ingredients.Where(i => usedIngredients.Any(ui => ui == i.IngredientId)).OrderBy(i => i.Name).ToList();
        }

        public async Task<int> CalculateDishPrice(ICollection<Ingredient> ingredients, int baseDishId)
        {
            var baseDish = await _baseDishService.GetBaseDishWithIngredients(baseDishId);
            var basedishIngredients = baseDish.BaseDishIngredients.Select(i => i.Ingredient).ToList();
            var addedIngredients = ingredients.Where(i => !basedishIngredients.Select(bdi => bdi.IngredientId).Contains(i.IngredientId)).ToList();
            return addedIngredients.Sum(ai => ai.AddExtraPrice) + baseDish.Price;
        }

        public async Task<bool> NotInBaseDish(Ingredient ingredient, int baseDishId)
        {
            var baseDish = await _baseDishService.GetBaseDishWithIngredients(baseDishId);
            return baseDish.BaseDishIngredients.All(bdi => bdi.Ingredient != ingredient);
        }

        public ICollection<Ingredient> AddIngredientToList(ISession session, int ingredientId)
        {
            List<Ingredient> IngredientsList = JsonConvert.DeserializeObject<List<Ingredient>>(session.GetString(Constants.IngredientsSession));
            IngredientsList.Add(_context.Ingredients.SingleOrDefault(i => i.IngredientId == ingredientId));
            return IngredientsList;
        }

        public ICollection<Ingredient> RemoveIngredientFromList(ISession session, int ingredientId)
        {
            List<Ingredient> IngredientsList = JsonConvert.DeserializeObject<List<Ingredient>>(session.GetString(Constants.IngredientsSession));
            IngredientsList.Remove(IngredientsList.SingleOrDefault(il => il.IngredientId == ingredientId));
            return IngredientsList;
        }
    }
}
