using System.Collections.Generic;
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

        public async Task<ICollection<BaseDish>> GetAllBaseDishesWithIngredients()
        {
            return await _context.BaseDishes
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
