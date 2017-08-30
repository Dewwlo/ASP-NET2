using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using HäggesPizzeria.Models.IngredientViewModels;
using HäggesPizzeria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HäggesPizzeria.Controllers
{
    public class IngredientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IngredientService _ingredientService;

        public IngredientController(ApplicationDbContext context, IngredientService ingredientService)
        {
            _context = context;
            _ingredientService = ingredientService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ingredients.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEditIngredient(int? ingredientId)
        {
            if (ingredientId == null)
            {
                return PartialView("_IngredientCreateEditPartial", new Ingredient());
            }
            
            var ingredient = await _context.Ingredients.SingleOrDefaultAsync(i => i.IngredientId == ingredientId);
            return PartialView("_IngredientCreateEditPartial", ingredient);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveIngredient(int id, [Bind("IngredientId,Name,AddExtraPrice,IsActive")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                if (id != 0)
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
            return PartialView("_IngredientCreateEditPartial", ingredient);
        }

        [HttpPost]
        public IActionResult UpdateDishIngredient(int baseDishId, int ingredientId, bool addIngredient, bool isOrderedDish)
        {
            var ingredientsList = addIngredient
                ? _ingredientService.AddIngredientToList(HttpContext, ingredientId)
                : _ingredientService.RemoveIngredientFromList(HttpContext, ingredientId);
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
