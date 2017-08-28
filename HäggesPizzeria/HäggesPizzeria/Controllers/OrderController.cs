using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace HäggesPizzeria.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Create()
        {
            var order = new Order();
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                order = new Order
                {
                    Email = user.Email,
                    Adress = user.Adress,
                    Zipcode = user.Zipcode,
                    PhoneNumber = user.PhoneNumber
                };
            }

            return View("OrderPayment", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,Adress,PhoneNumber,Zipcode")] Order order)
        {
            if (ModelState.IsValid)
            {
                var sessionCart = HttpContext.Session.GetString("Cart");

                if (sessionCart != null)
                {
                    SaveOrder(order);
                    await SaveOrderedDishes(JsonConvert.DeserializeObject<List<OrderedDish>>(sessionCart));
                    HttpContext.Session.Remove("Cart");
                }

                return RedirectToAction("Index", "Home");
            }

            return View("OrderPayment", order);
        }

        public void SaveOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public async Task SaveOrderedDishes(ICollection<OrderedDish> orderedDishes)
        {
            var order = _context.Orders.OrderByDescending(o => o.OrderId).FirstOrDefault();
            order.OrderDate = DateTime.Now;
            order.User = await _userManager.GetUserAsync(HttpContext.User);
            order.TotalPrice = orderedDishes.Sum(od => od.Price);
            CreateOrderedDishes(orderedDishes, order);
        }

        public void CreateOrderedDishes(ICollection<OrderedDish> orderedDishes, Order order)
        {
            foreach (var orderedDish in orderedDishes)
            {
                _context.OrderedDishes.Add(orderedDish);
                _context.SaveChanges();

                var newOrderedDish = _context.OrderedDishes.OrderByDescending(od => od.OrderedDishId).FirstOrDefault();
                newOrderedDish.Order = order;

                _context.OrderedDishIngredients.AddRange(orderedDishes.Select(od => new OrderedDishIngredient { OrderedDishId = newOrderedDish.OrderedDishId, IngredientId = od.OrderedDishId }).ToList());
                _context.SaveChanges();
            }
        }
    }
}
