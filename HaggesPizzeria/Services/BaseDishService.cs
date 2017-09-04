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

        public BaseDishService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<BaseDish>> GetAllBaseDishes()
        {
            return await _context.BaseDishes.Include(bd => bd.Category).ToListAsync();
        }

        public async Task<ICollection<BaseDish>> GetAllActiveBaseDishesWithIngredients()
        {
            return await _context.BaseDishes.Where(bd => bd.IsActive)
                .Include(d => d.BaseDishIngredients)
                .ThenInclude(di => di.Ingredient)
                .ToListAsync();
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
            return await _context.BaseDishes
                .Where(bd => bd.IsActive && bd.Category.CategoryId == categoryId)
                .Include(d => d.BaseDishIngredients)
                .ThenInclude(di => di.Ingredient)
                .ToListAsync();
        }

        public void SaveIngredientsToDish(HttpContext httpContext)
        {
            var ingredientsListSession = httpContext.Session.GetString("IngredientsList");
            var ingredients = (ingredientsListSession != null)
                ? JsonConvert.DeserializeObject<List<Ingredient>>(ingredientsListSession)
                : new List<Ingredient>();
            var baseDish = _context.BaseDishes.OrderByDescending(bd => bd.BaseDishId).FirstOrDefault();

            _context.BaseDishIngredients.AddRange(ingredients.Select(i => new BaseDishIngredient { IngredientId = i.IngredientId, BaseDishId = baseDish.BaseDishId }));
            _context.SaveChanges();
        }


        public void SaveIngredientsToDish(HttpContext httpContext, int baseDishId)
        {
            List<Ingredient> ingredientsList = JsonConvert.DeserializeObject<List<Ingredient>>(httpContext.Session.GetString("IngredientsList"));
            _context.BaseDishIngredients.RemoveRange(_context.BaseDishIngredients.Where(bdi => bdi.BaseDishId == baseDishId));
            _context.SaveChanges();
            _context.BaseDishIngredients.AddRange(ingredientsList.Select(il => new BaseDishIngredient { BaseDishId = baseDishId, IngredientId = il.IngredientId }).ToList());
            _context.SaveChanges();
        }
    }
}
