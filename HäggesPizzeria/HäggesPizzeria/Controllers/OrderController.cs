using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using HäggesPizzeria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace HäggesPizzeria.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PaymentService _paymentService;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, PaymentService paymentService)
        {
            _context = context;
            _userManager = userManager;
            _paymentService = paymentService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderedDishes)
                .ThenInclude(od => od.OrderedDishIngredients)
                .ThenInclude(odi => odi.Ingredient).ToListAsync());
        }

        [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> ValidateShippingInformation()
        {
            var order = new Order();
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                order.Email = user.Email;
                order.Adress = user.Adress;
                order.Zipcode = user.Zipcode;
                order.PhoneNumber = user.PhoneNumber;
            }

            return View("ShippingInformation", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ValidateShippingInformation([Bind("Email,Adress,PhoneNumber,Zipcode")] Order order)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("OrderInformation", JsonConvert.SerializeObject(order));
                return RedirectToAction("ValidatePayment");
            }

            return View("ShippingInformation", order);
        }

        public IActionResult ValidatePayment()
        {
            return View("Payment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ValidatePayment(Payment payment)
        {
            if (ModelState.IsValid && _paymentService.ValidatePaymentInformation(payment))
            {
                var sessionOrder = HttpContext.Session.GetString("OrderInformation");
                var sessionCart = HttpContext.Session.GetString("Cart");

                if (sessionCart != null && sessionOrder != null)
                {
                    SaveOrder(JsonConvert.DeserializeObject<Order>(sessionOrder));
                    SaveOrderedDishes(JsonConvert.DeserializeObject<List<OrderedDish>>(sessionCart));
                    HttpContext.Session.Remove("Cart");
                    HttpContext.Session.Remove("OrderInformation");
                }

                return RedirectToAction("Index", "Home");
            }

            return View("Payment");
        }

        public void SaveOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void SaveOrderedDishes(ICollection<OrderedDish> orderedDishes)
        {
            var order = _context.Orders.OrderByDescending(o => o.OrderId).FirstOrDefault();
            order.OrderDate = DateTime.Now;
            order.TotalPrice = orderedDishes.Sum(od => od.Price);
            order.User = _context.Users.SingleOrDefault(u => u.Email == order.Email);
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
