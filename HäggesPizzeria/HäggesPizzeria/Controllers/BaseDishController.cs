using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using Microsoft.AspNetCore.Authorization;
using HäggesPizzeria.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HäggesPizzeria.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BaseDishController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BaseDishService _baseDishService;

        public BaseDishController(ApplicationDbContext context, BaseDishService baseDishService)
        {
            _context = context;
            _baseDishService = baseDishService;
        }

        public async Task<IActionResult> Index()
        {
            HttpContext.Session.SetString("IngredientsList", JsonConvert.SerializeObject(new List<Ingredient>()));
            return View(await _baseDishService.GetAllBaseDishes());
        }

        public async Task<IActionResult> CreateEditBaseDish(int? basedishId)
        {
            if (basedishId == null)
            {
                // TODO Find another solution, ugly hack to be able to render view.
                return PartialView("_BaseDishCreateEditPartial", new BaseDish { BaseDishIngredients = new List<BaseDishIngredient>() });
            }
            else
            {
                var ingredientsList = _context.BaseDishIngredients.Where(bdi => bdi.BaseDishId == basedishId).Select(i => i.Ingredient).ToList();
                HttpContext.Session.SetString("IngredientsList", JsonConvert.SerializeObject(ingredientsList));
                var baseDish = await _baseDishService.GetBaseDishWithIngredients((int) basedishId);
                return PartialView("_BaseDishCreateEditPartial", baseDish);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBaseDish(int baseDishId, int categoryId, [Bind("BaseDishId,Name,Price,IsActive")] BaseDish baseDish)
        {
            if (ModelState.IsValid)
            {
                baseDish.Category = _context.Categories.SingleOrDefault(c => c.CategoryId == categoryId);

                // TODO Another way to differ between create/edit mode
                if (baseDishId != 0)
                {
                    _context.Update(baseDish);
                    await _context.SaveChangesAsync();
                    UpdateBaseDishIngredient(baseDish.BaseDishId);
                }
                else
                {
                    _context.Add(baseDish);
                    await _context.SaveChangesAsync();
                    SaveIngredientsToDish();
                }
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_BaseDishCreateEditPartial", baseDish);
        }

        private void SaveIngredientsToDish()
        {
            var ingredientsListSession = HttpContext.Session.GetString("IngredientsList");
            var ingredients = (ingredientsListSession != null)
                ? JsonConvert.DeserializeObject<List<Ingredient>>(ingredientsListSession)
                : new List<Ingredient>();
            var baseDish = _context.BaseDishes.OrderByDescending(bd => bd.BaseDishId).FirstOrDefault();

            _context.BaseDishIngredients.AddRange(ingredients.Select(i => new BaseDishIngredient { IngredientId = i.IngredientId, BaseDishId = baseDish.BaseDishId }));
            _context.SaveChanges();
        }

        private void UpdateBaseDishIngredient(int baseDishId)
        {
            List<Ingredient> ingredientsList = JsonConvert.DeserializeObject<List<Ingredient>>(HttpContext.Session.GetString("IngredientsList"));
            _context.BaseDishIngredients.RemoveRange(_context.BaseDishIngredients.Where(bdi => bdi.BaseDishId == baseDishId));
            _context.SaveChanges();
            _context.BaseDishIngredients.AddRange(ingredientsList.Select(il => new BaseDishIngredient { BaseDishId = baseDishId, IngredientId = il.IngredientId }).ToList());
            _context.SaveChanges();
        }
    }
}
