using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaggesPizzeria.Data;
using HaggesPizzeria.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HaggesPizzeria.Services
{
    public class BaseDishService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISession _session;

        public BaseDishService(ApplicationDbContext context, ISession session)
        {
            _context = context;
            _session = session;
        }

        public async Task<ICollection<BaseDish>> GetAllBaseDishes()
        {
            return await _context.BaseDishes.Include(bd => bd.Category).ToListAsync();
        }

        public async Task<ICollection<BaseDish>> GetAllActiveBaseDishesWithIngredients()
        {
            var baseDishes = await _context.BaseDishes.Where(bd => bd.IsActive)
                .Include(d => d.BaseDishIngredients)
                .ThenInclude(di => di.Ingredient)
                .ToListAsync();
            return FilterOutDishesWithInactiveIngredients(baseDishes);
        }

        public async Task<BaseDish> GetBaseDishWithIngredients(int id)
        {
            return await _context.BaseDishes
                .Include(d => d.BaseDishIngredients)
                .ThenInclude(di => di.Ingredient)
                .Include(c => c.Category)
                .SingleOrDefaultAsync(m => m.BaseDishId == id);
        }

        public async Task<ICollection<BaseDish>> GetAllActiveBaseDishesWithIngredientsByCategory(int categoryId)
        {
            var baseDishes = await _context.BaseDishes
                .Where(bd => bd.IsActive && bd.Category.CategoryId == categoryId)
                .Include(d => d.BaseDishIngredients)
                .ThenInclude(di => di.Ingredient)
                .ToListAsync();
            return FilterOutDishesWithInactiveIngredients(baseDishes);
        }

        public void SaveIngredientsToDish()
        {
            var ingredientsListSession = _session.GetString(Constants.IngredientsSession);
            var ingredients = (ingredientsListSession != null)
                ? JsonConvert.DeserializeObject<List<Ingredient>>(ingredientsListSession)
                : new List<Ingredient>();
            var baseDish = _context.BaseDishes.OrderByDescending(bd => bd.BaseDishId).FirstOrDefault();

            _context.BaseDishIngredients.AddRange(ingredients.Select(i => new BaseDishIngredient { IngredientId = i.IngredientId, BaseDishId = baseDish.BaseDishId }));
            _context.SaveChanges();
        }


        public void SaveIngredientsToDish(int baseDishId)
        {
            List<Ingredient> ingredientsList = JsonConvert.DeserializeObject<List<Ingredient>>(_session.GetString(Constants.IngredientsSession));
            _context.BaseDishIngredients.RemoveRange(_context.BaseDishIngredients.Where(bdi => bdi.BaseDishId == baseDishId));
            _context.SaveChanges();
            _context.BaseDishIngredients.AddRange(ingredientsList.Select(il => new BaseDishIngredient { BaseDishId = baseDishId, IngredientId = il.IngredientId }).ToList());
            _context.SaveChanges();
        }

        public List<BaseDish> FilterOutDishesWithInactiveIngredients(List<BaseDish> baseDishes)
        {
            return baseDishes.Where(bd => bd.BaseDishIngredients.All(bdi => bdi.Ingredient.IsActive)).ToList();
        }
    }
}
