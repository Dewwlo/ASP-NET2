using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using HäggesPizzeria.Services;
using Microsoft.AspNetCore.Authorization;

namespace HäggesPizzeria.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CategoryService _categoryService;

        public CategoryController(ApplicationDbContext context, CategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _categoryService.GetAllCategories());
        }

        public async Task<IActionResult> CreateEditCategory(int? categoryId)
        {
            if (categoryId == null)
            {
                return PartialView("_CategoryCreateEditPartial", new Category());
            }
            else
            {
                var category = await _context.Categories.SingleOrDefaultAsync(c => c.CategoryId == categoryId);
                return PartialView("_CategoryCreateEditPartial", category);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCategory(int? id, [Bind("CategoryId,Name,IsActive")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (id != null)
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_CategoryCreateEditPartial", category);
        }
    }
}
