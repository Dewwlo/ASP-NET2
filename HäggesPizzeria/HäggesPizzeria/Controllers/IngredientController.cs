using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IngredientId,Name,AddExtraPrice,IsActive")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ingredient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("Index", ingredient);
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
    }
}
