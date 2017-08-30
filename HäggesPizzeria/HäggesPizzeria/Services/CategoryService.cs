using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HäggesPizzeria.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<ICollection<Category>> GetAllActiveCategories()
        {
            return await _context.Categories.Where(c => c.IsActive).ToListAsync();
        }

        public async Task<ICollection<SelectListItem>> GetAllCategorySelectOptions()
        {
            return await _context.Categories.Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name }).ToListAsync();
        }
    }
}
