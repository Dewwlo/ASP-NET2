using System.Diagnostics;
using System.Threading.Tasks;
using HäggesPizzeria.Data;
using Microsoft.AspNetCore.Mvc;
using HäggesPizzeria.Models;
using Microsoft.EntityFrameworkCore;
using HäggesPizzeria.Services;

namespace HäggesPizzeria.Controllers
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

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
