using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaggesPizzeria.Data;
using HaggesPizzeria.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HaggesPizzeria.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IngredientService _ingredientService;
        private readonly BaseDishService _baseDishService;

        public CartService(ApplicationDbContext context, IngredientService ingredientService, BaseDishService baseDishService)
        {
            _context = context;
            _ingredientService = ingredientService;
            _baseDishService = baseDishService;
        }

        public CartDetails GetCartDetails(HttpContext httpContext)
        {
            var sessionCart = httpContext.Session.GetString("Cart");

            if (sessionCart != null)
            {
                return CalculateCartDetails(GetSessionCartList(httpContext, "Cart"));
            }

            return new CartDetails();
        }

        public async Task AddDishToCart(HttpContext httpContext, int dishId)
        {
            var sessionCart = httpContext.Session.GetString("Cart");
            ICollection<OrderedDish> cart = (sessionCart != null)
                ? GetSessionCartList(httpContext, "Cart")
                : new List<OrderedDish>();

            cart.Add(CopyBaseDishToOrderedDish(await _baseDishService.GetBaseDishWithIngredients(dishId)));
            SetSessionCartList(httpContext, "Cart", cart);
        }

        public void RemoveDishFromCart(HttpContext httpContext, Guid guid)
        {
            var sessionCart = httpContext.Session.GetString("Cart");

            if (sessionCart != null)
            {
                var cart = GetSessionCartList(httpContext, "Cart");
                cart.Remove(cart.SingleOrDefault(d => d.Guid == guid));
                SetSessionCartList(httpContext, "Cart", cart);
            }
        }

        public bool CartHasItems(HttpContext httpContext)
        {
            return httpContext.Session.GetString("Cart").Any();
        }

        public OrderedDish GetDishDetails(HttpContext httpContext, Guid guid)
        {
            var dish = GetSessionCartList(httpContext, "Cart").SingleOrDefault(c => c.Guid == guid);
            SetSessionIngredientsList(httpContext, "IngredientsList", _context.Ingredients.Where(i => dish.Ingredients.Any(di => di == i.IngredientId)).ToList());
            return dish;
        }

        public async Task<ICollection<OrderedDish>> SaveDishIngredients(HttpContext httpContext, Guid guid)
        {
            var cart = GetSessionCartList(httpContext, "Cart");
            var dish = cart.FirstOrDefault(d => d.Guid == guid);
            var ingredients = GetSessionIngredientsList(httpContext, "IngredientsList").ToList();
            dish.Ingredients = ingredients.Select(i => i.IngredientId).ToList();
            dish.Price = await _ingredientService.CalculateDishPrice(ingredients, dish.BaseDishId);
            SetSessionCartList(httpContext, "Cart", cart);

            return cart;
        }

        public ICollection<OrderedDish> GetSessionCartList(HttpContext httpContext, string sessionName)
        {
            return JsonConvert.DeserializeObject<List<OrderedDish>>(httpContext.Session.GetString(sessionName));
        }

        public ICollection<Ingredient> GetSessionIngredientsList(HttpContext httpContext, string sessionName)
        {
            return JsonConvert.DeserializeObject<List<Ingredient>>(httpContext.Session.GetString(sessionName));
        }

        public void SetSessionCartList(HttpContext httpContext, string sessionName, ICollection<OrderedDish> list)
        {
            httpContext.Session.SetString(sessionName, JsonConvert.SerializeObject(list));
        }

        public void SetSessionIngredientsList(HttpContext httpContext ,string sessionName, ICollection<Ingredient> list)
        {
            httpContext.Session.SetString(sessionName, JsonConvert.SerializeObject(list));
        }

        private OrderedDish CopyBaseDishToOrderedDish(BaseDish baseDish)
        {
            return new OrderedDish
            {
                Name = baseDish.Name,
                BaseDishId = baseDish.BaseDishId,
                Price = baseDish.Price,
                Ingredients = baseDish.BaseDishIngredients.Select(bdi => bdi.Ingredient.IngredientId).ToList(),
                Guid = Guid.NewGuid()
            };
        }

        private CartDetails CalculateCartDetails(ICollection<OrderedDish> orderedDishes)
        {
            var cartDetails = new CartDetails
            {
                Items = orderedDishes.Count,
                TotalPrice = orderedDishes.Sum(od => od.Price),
            };

            foreach (var orderedDish in orderedDishes)
            {
                cartDetails.BaseDishesPrice += _context.BaseDishes.SingleOrDefault(bd => bd.BaseDishId == orderedDish.BaseDishId).Price;
            }
            cartDetails.AddedIngredientsPrice = cartDetails.TotalPrice - cartDetails.BaseDishesPrice;

            return cartDetails;
        }
    }
}
