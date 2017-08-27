using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using HäggesPizzeria.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HäggesPizzeria.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BaseDishService _baseDishService;

        public CartController(ApplicationDbContext context, BaseDishService baseDishService)
        {
            _context = context;
            _baseDishService = baseDishService;
        }

        public async Task<IActionResult> AddDishToCart(int dishId)
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            List<OrderedDish> cart = (sessionCart != null)
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

        public IActionResult DishDetails(Guid guid)
        {
            List<OrderedDish> cart = JsonConvert.DeserializeObject<List<OrderedDish>>(HttpContext.Session.GetString("Cart"));
            var dish = cart.SingleOrDefault(c => c.Guid == guid);
            HttpContext.Session.SetString("IngredientsList", JsonConvert.SerializeObject(_context.Ingredients.Where(i => dish.Ingredients.Any(di => di == i.IngredientId))));
            return View("CartDishDetails", dish);
        }

        //public IActionResult EditDishIngredients(Guid guid)
        //{
        //    var sessionCart = HttpContext.Session.GetString("Cart");
        //        List<OrderedDish> cart = JsonConvert.DeserializeObject<List<OrderedDish>>(sessionCart);
        //        cart.Remove(cart.SingleOrDefault(d => d.Guid == guid));
        //        HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        //        return View("Cart", cart);
        //    }

        //    return RedirectToAction("Index", "Home");
        //}
    }
}