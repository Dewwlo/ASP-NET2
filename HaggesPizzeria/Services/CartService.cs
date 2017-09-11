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

        public CartDetails GetCartDetails(ISession session)
        {
            var sessionCart = session.GetString(Constants.CartSession);

            if (sessionCart != null)
            {
                return CalculateCartDetails(GetSessionCartList(session, Constants.CartSession));
            }

            return new CartDetails();
        }

        public async Task AddDishToCart(ISession session, int dishId)
        {
            var sessionCart = session.GetString(Constants.CartSession);
            ICollection<OrderedDish> cart = (sessionCart != null)
                ? GetSessionCartList(session, Constants.CartSession)
                : new List<OrderedDish>();

            cart.Add(CopyBaseDishToOrderedDish(await _baseDishService.GetBaseDishWithIngredients(dishId)));
            SetSessionCartList(session, Constants.CartSession, cart);
        }

        public void RemoveDishFromCart(ISession session, Guid guid)
        {
            var sessionCart = session.GetString(Constants.CartSession);

            if (sessionCart != null)
            {
                var cart = GetSessionCartList(session, Constants.CartSession);
                cart.Remove(cart.SingleOrDefault(d => d.Guid == guid));
                SetSessionCartList(session, Constants.CartSession, cart);
            }
        }

        public bool CartHasItems(ISession session)
        {
            return session.GetString(Constants.CartSession).Any();
        }

        public OrderedDish GetDishDetails(ISession session, Guid guid)
        {
            var dish = GetSessionCartList(session, Constants.CartSession).SingleOrDefault(c => c.Guid == guid);
            SetSessionIngredientsList(session, Constants.IngredientsSession, _context.Ingredients.Where(i => dish.Ingredients.Any(di => di == i.IngredientId)).ToList());
            return dish;
        }

        public async Task<ICollection<OrderedDish>> SaveDishIngredients(ISession session, Guid guid)
        {
            var cart = GetSessionCartList(session, Constants.CartSession);
            var dish = cart.FirstOrDefault(d => d.Guid == guid);
            var ingredients = GetSessionIngredientsList(session, Constants.IngredientsSession).ToList();
            dish.Ingredients = ingredients.Select(i => i.IngredientId).ToList();
            dish.Price = await _ingredientService.CalculateDishPrice(ingredients, dish.BaseDishId);
            SetSessionCartList(session, Constants.CartSession, cart);

            return cart;
        }

        public ICollection<OrderedDish> GetSessionCartList(ISession session, string sessionName)
        {
            return JsonConvert.DeserializeObject<List<OrderedDish>>(session.GetString(sessionName));
        }

        public ICollection<Ingredient> GetSessionIngredientsList(ISession session, string sessionName)
        {
            return JsonConvert.DeserializeObject<List<Ingredient>>(session.GetString(sessionName));
        }

        public void SetSessionCartList(ISession session, string sessionName, ICollection<OrderedDish> list)
        {
            session.SetString(sessionName, JsonConvert.SerializeObject(list));
        }

        public void SetSessionIngredientsList(ISession session ,string sessionName, ICollection<Ingredient> list)
        {
            session.SetString(sessionName, JsonConvert.SerializeObject(list));
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
