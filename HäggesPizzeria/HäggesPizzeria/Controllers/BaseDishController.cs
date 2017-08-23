using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;

namespace HäggesPizzeria.Controllers
{
    public class BaseDishController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaseDishController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dish
        public async Task<IActionResult> Index()
        {
            return View(await _context.BaseDishes.ToListAsync());
        }

        // GET: Dish/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.BaseDishes
                .Include(d => d.BaseDishIngredients)
                .ThenInclude(di => di.Ingredient)
                .SingleOrDefaultAsync(m => m.BaseDishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // GET: Dish/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dish/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DishId,Name,Price")] BaseDish dish)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dish);
        }

        // GET: Dish/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.BaseDishes
                .Include(d => d.BaseDishIngredients)
                .ThenInclude(d => d.Ingredient)
                .SingleOrDefaultAsync(m => m.BaseDishId == id);
            if (dish == null)
            {
                return NotFound();
            }
            return View(dish);
        }

        // POST: Dish/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BaseDishId,Name,Price")] BaseDish dish)
        {
            if (id != dish.BaseDishId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.BaseDishId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dish);
        }

        // GET: Dish/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.BaseDishes
                .SingleOrDefaultAsync(m => m.BaseDishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // POST: Dish/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.BaseDishes.SingleOrDefaultAsync(m => m.BaseDishId == id);
            _context.BaseDishes.Remove(dish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            return _context.BaseDishes.Any(e => e.BaseDishId == id);
        }

        public ActionResult UpdateBaseDishIngredient(int dishId, int ingredientId, bool addIngredient)
        {
            var result = addIngredient ?
                _context.BaseDishIngredients.Add(new BaseDishIngredient() { BaseDishId = dishId, IngredientId = ingredientId}) :
                _context.BaseDishIngredients.Remove(new BaseDishIngredient() {BaseDishId = dishId, IngredientId = ingredientId});
            _context.SaveChanges();

            return PartialView("_IngredientPartial", _context.BaseDishes
                    .Include(d => d.BaseDishIngredients)
                    .ThenInclude(di => di.Ingredient)
                    .SingleOrDefault(d => d.BaseDishId == dishId));
        }
    }
}
