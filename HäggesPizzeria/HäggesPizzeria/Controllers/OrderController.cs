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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using Newtonsoft.Json;

namespace HäggesPizzeria.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BaseDishService _baseDishService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, BaseDishService baseDishService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _baseDishService = baseDishService;
            _userManager = userManager;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: Order/Details/5
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

        // GET: Order/Create
        public async Task<IActionResult> Create()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");

            if (sessionCart != null)
            {
                SaveOrder();
                await SaveOrderedDishes(JsonConvert.DeserializeObject<List<OrderedDish>>(sessionCart));
                HttpContext.Session.Remove("Cart");
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> AddDishToCart(int dishId)
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            List <OrderedDish> cart = (sessionCart != null) 
                ? JsonConvert.DeserializeObject<List<OrderedDish>>(sessionCart)
                : new List<OrderedDish>();

            cart.Add(CopyBaseDishToOrderedDish(await _baseDishService.GetBaseDishWithIngredients(dishId)));
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));

            return RedirectToAction("Index", "Home");
        }

        public IActionResult RemoveDishFromCart(Guid guid)
        {
            var sessionCart = HttpContext.Session.GetString("Cart");

            if (sessionCart != null)
            {
                List<OrderedDish> cart = JsonConvert.DeserializeObject<List<OrderedDish>>(sessionCart);
                cart.Remove(cart.SingleOrDefault(d => d.Guid == guid));
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
                return View("Cart", cart);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult GetAllCartItems()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");

            if (sessionCart != null)
            {
                List<OrderedDish> cart = JsonConvert.DeserializeObject<List<OrderedDish>>(sessionCart);
                return View("Cart", cart);
            }

            return View("Cart");
        }

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,TotalPrice")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public OrderedDish CopyBaseDishToOrderedDish(BaseDish baseDish)
        {
            return new OrderedDish
            {
                Name = baseDish.Name,
                Price = baseDish.Price,
                Ingredients = baseDish.BaseDishIngredients.Select(bdi => bdi.Ingredient.IngredientId).ToList(),
                Guid = Guid.NewGuid()
            };
        }

        public void SaveOrder()
        {
            _context.Orders.Add(new Order{OrderDate = DateTime.Now});
            _context.SaveChanges();
        }

        public async Task SaveOrderedDishes(ICollection<OrderedDish> orderedDishes)
        {
            var order = _context.Orders.OrderByDescending(o => o.OrderId).FirstOrDefault();
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
                foreach (var ingredient in orderedDish.Ingredients)
                {
                    _context.OrderedDishIngredients.Add(new OrderedDishIngredient { OrderedDishId = newOrderedDish.OrderedDishId, IngredientId = ingredient});
                    _context.SaveChanges();
                }
            }
        }
    }
}
