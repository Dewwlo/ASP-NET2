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
        private readonly ISession _session;

        public CartService(
            ApplicationDbContext context, 
            IngredientService ingredientService, 
            BaseDishService baseDishService, 
            ISession session)
        {
            _context = context;
            _ingredientService = ingredientService;
            _baseDishService = baseDishService;
            _session = session;
        }

        public CartDetails GetCartDetails()
        {
            var sessionCart = _session.GetString(Constants.CartSession);

            if (sessionCart != null)
            {
                return CalculateCartDetails(GetSessionCartList(Constants.CartSession));
            }

            return new CartDetails();
        }

        public async Task AddDishToCart(int dishId)
        {
            var sessionCart = _session.GetString(Constants.CartSession);
            ICollection<OrderedDish> cart = (sessionCart != null)
                ? GetSessionCartList(Constants.CartSession)
                : new List<OrderedDish>();

            cart.Add(CopyBaseDishToOrderedDish(await _baseDishService.GetBaseDishWithIngredients(dishId)));
            SetSessionCartList(Constants.CartSession, cart);
        }

        public void AddDishToCart(BaseDish baseDish)
        {
            var sessionCart = _session.GetString(Constants.CartSession);
            ICollection<OrderedDish> cart = (sessionCart != null)
                ? GetSessionCartList(Constants.CartSession)
                : new List<OrderedDish>();

            cart.Add(CopyBaseDishToOrderedDish(baseDish));
            SetSessionCartList(Constants.CartSession, cart);
        }

        public void RemoveDishFromCart(Guid guid)
        {
            var sessionCart = _session.GetString(Constants.CartSession);

            if (sessionCart != null)
            {
                var cart = GetSessionCartList(Constants.CartSession);
                cart.Remove(cart.SingleOrDefault(d => d.Guid == guid));
                SetSessionCartList(Constants.CartSession, cart);
            }
        }

        public bool CartHasItems()
        {
            return _session.GetString(Constants.CartSession).Any();
        }

        public OrderedDish GetDishDetails(ISession session, Guid guid)
        {
            var dish = GetSessionCartList(Constants.CartSession).SingleOrDefault(c => c.Guid == guid);
            SetSessionIngredientsList(Constants.IngredientsSession, _context.Ingredients.Where(i => dish.Ingredients.Any(di => di == i.IngredientId)).ToList());
            return dish;
        }

        public async Task<ICollection<OrderedDish>> SaveDishIngredients(Guid guid)
        {
            var cart = GetSessionCartList(Constants.CartSession);
            var dish = cart.FirstOrDefault(d => d.Guid == guid);
            var ingredients = GetSessionIngredientsList(Constants.IngredientsSession).ToList();
            dish.Ingredients = ingredients.Select(i => i.IngredientId).ToList();
            dish.Price = await _ingredientService.CalculateDishPrice(ingredients, dish.BaseDishId);
            SetSessionCartList(Constants.CartSession, cart);

            return cart;
        }

        public ICollection<OrderedDish> GetSessionCartList(string sessionName)
        {
            return JsonConvert.DeserializeObject<List<OrderedDish>>(_session.GetString(sessionName));
        }

        public ICollection<Ingredient> GetSessionIngredientsList(string sessionName)
        {
            return JsonConvert.DeserializeObject<List<Ingredient>>(_session.GetString(sessionName));
        }

        public void SetSessionCartList(string sessionName, ICollection<OrderedDish> list)
        {
            _session.SetString(sessionName, JsonConvert.SerializeObject(list));
        }

        public void SetSessionIngredientsList(string sessionName, ICollection<Ingredient> list)
        {
            _session.SetString(sessionName, JsonConvert.SerializeObject(list));
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
