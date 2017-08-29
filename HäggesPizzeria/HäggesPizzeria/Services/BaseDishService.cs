using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using Microsoft.EntityFrameworkCore;

namespace HäggesPizzeria.Services
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
            return await _context.BaseDishes.ToListAsync();
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
                .SingleOrDefaultAsync(m => m.BaseDishId == id);
        }
    }
}
