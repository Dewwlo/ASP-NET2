using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using HäggesPizzeria.Models.IngredientViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HäggesPizzeria.Controllers
{
    public class IngredientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngredientController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Ingredients.ToListAsync());
        }

        public async Task<IActionResult> CreateEditIngredient(int? ingredientId)
        {
            if (ingredientId == null)
            {
                return PartialView("_CreateEditIngredientPartial", new Ingredient());
            }
            else
            {
                var ingredient = await _context.Ingredients.SingleOrDefaultAsync(m => m.IngredientId == ingredientId);
                return PartialView("_CreateEditIngredientPartial", ingredient);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveIngredient(int? id, [Bind("IngredientId,Name,AddExtraPrice,IsActive")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                if (id != null)
                {
                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Add(ingredient);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_CreateEditIngredientPartial", ingredient);
        }

        [HttpPost]
        public IActionResult UpdateDishIngredient(int baseDishId, int ingredientId, bool addIngredient, bool isOrderedDish)
        {
            List<Ingredient> ingredientsList = JsonConvert.DeserializeObject<List<Ingredient>>(HttpContext.Session.GetString("IngredientsList"));

            if (addIngredient)
                ingredientsList.Add(_context.Ingredients.SingleOrDefault(i => i.IngredientId == ingredientId));
            else
                ingredientsList.Remove(ingredientsList.SingleOrDefault(il => il.IngredientId == ingredientId));

            HttpContext.Session.SetString("IngredientsList", JsonConvert.SerializeObject(ingredientsList));

            return PartialView("_IngredientPartial", new IngedientDishViewModel
            {
                BaseDishId = baseDishId,
                Ingredients = ingredientsList,
                IsOrderedDish = isOrderedDish
            });
        }
    }
}
