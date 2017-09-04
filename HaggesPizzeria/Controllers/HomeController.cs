using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HaggesPizzeria.Models;
using HaggesPizzeria.Services;

namespace HaggesPizzeria.Controllers
{
    public class HomeController : Controller
    {
        private readonly BaseDishService _baseDishService;

        public HomeController(BaseDishService baseDishService)
        {
            _baseDishService = baseDishService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _baseDishService.GetAllActiveBaseDishesWithIngredients());
        }

        public async Task<IActionResult> GetDishesByCategory(int categoryId)
        {
            return View("Index", await _baseDishService.GetAllActiveBaseDishesWithIngredientsByCategory(categoryId));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
