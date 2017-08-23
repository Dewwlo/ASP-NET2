using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using Microsoft.EntityFrameworkCore;

namespace HäggesPizzeria.Services
{
    public class IngredientService
    {
        private readonly ApplicationDbContext _context;

        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<Ingredient> GetAllActiveIngredients()
        {
            return _context.Ingredients.ToList();
        }
    }
}
