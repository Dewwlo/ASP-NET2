﻿using System.Collections.Generic;
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
            return View(await _baseDishService.GetAllBaseDishes());
        }

        public IActionResult Create()
        {
            HttpContext.Session.SetString("IngredientsList", JsonConvert.SerializeObject(new List<Ingredient>()));
            return View();
        }

        public async Task<IActionResult> CreateEditBaseDish(int? basedishId)
        {
            if (basedishId == null)
            {
                return PartialView("_BaseDishCreateEditPartial", new BaseDish());
            }
            else
            {
                var baseDish = await _baseDishService.GetBaseDishWithIngredients((int) basedishId);
                return PartialView("_BaseDishCreateEditPartial", baseDish);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBaseDish(int? id, int categoryId, [Bind("BaseDishId,Name,Price,IsActive")] BaseDish baseDish)
        {
            if (ModelState.IsValid)
            {
                if (id != null)
                {
                    baseDish.Category = _context.Categories.SingleOrDefault(c => c.CategoryId == categoryId);
                    _context.Update(baseDish);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    baseDish.Category = _context.Categories.SingleOrDefault(c => c.CategoryId == categoryId);
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

        public async Task<IActionResult> Edit(int id)
        {
            var ingredientsList = _context.BaseDishIngredients.Where(bdi => bdi.BaseDishId == id).Select(i => i.Ingredient).ToList();
            HttpContext.Session.SetString("IngredientsList", JsonConvert.SerializeObject(ingredientsList));

            return View(await _baseDishService.GetBaseDishWithIngredients(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BaseDishId,Name,Price,IsActive")] BaseDish dish)
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
                    UpdateBaseDishIngredient(dish.BaseDishId);
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

        private void UpdateBaseDishIngredient(int baseDishId)
        {
            List<Ingredient> ingredientsList = JsonConvert.DeserializeObject<List<Ingredient>>(HttpContext.Session.GetString("IngredientsList"));
            _context.BaseDishIngredients.RemoveRange(_context.BaseDishIngredients.Where(bdi => bdi.BaseDishId == baseDishId));
            _context.SaveChanges();
            _context.BaseDishIngredients.AddRange(ingredientsList.Select(il => new BaseDishIngredient { BaseDishId = baseDishId, IngredientId = il.IngredientId }).ToList());
            _context.SaveChanges();
        }

        private bool DishExists(int id)
        {
            return _context.BaseDishes.Any(e => e.BaseDishId == id);
        }
    }
}
