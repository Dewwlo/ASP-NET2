using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaggesPizzeria.Data;
using HaggesPizzeria.Models;
using HaggesPizzeria.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HaggesPizzeriaTest
{
    [TestClass]
    public class CartServiceTest : BaseTests
    {
        public override void InitializeDatabase()
        {
            base.InitializeDatabase();

            var context = _serviceProvider.GetService<ApplicationDbContext>();
            context.Ingredients.Add(new Ingredient {Name = "AAA", AddExtraPrice = 5});
            context.Ingredients.Add(new Ingredient {Name = "BBB", AddExtraPrice = 10});
            context.Ingredients.Add(new Ingredient { Name = "CCC", AddExtraPrice = 5 });
            context.BaseDishes.Add(new BaseDish {Name = "Test", Price = 90});
            context.BaseDishes.Add(new BaseDish { Name = "Test2", Price = 100 });
            context.BaseDishIngredients.Add(new BaseDishIngredient {IngredientId = 3, BaseDishId = 1});
            context.SaveChanges();
        }
        
        [TestMethod]
        public async Task CompleteCartCalculationIntegrationTest()
        {
            var cartService = _serviceProvider.GetService<CartService>();
            var ingredientService = _serviceProvider.GetService<IngredientService>();

            await cartService.AddDishToCart(1);
            await cartService.AddDishToCart(2);
            cartService.SetSessionIngredientsList(Constants.IngredientsSession, new List<Ingredient>());

            var ingredient1 = ingredientService.AddIngredientToList(1);
            cartService.SetSessionIngredientsList(Constants.IngredientsSession, ingredient1);
            var ingredient2 = ingredientService.AddIngredientToList(2);
            cartService.SetSessionIngredientsList(Constants.IngredientsSession, ingredient2);
            
            var cart = cartService.GetSessionCartList(Constants.CartSession);
            await cartService.SaveDishIngredients(cart.FirstOrDefault().Guid);

            var sumCart = cartService.GetSessionCartList(Constants.CartSession).Sum(od => od.Price);

            Assert.AreEqual(205, sumCart);
            Assert.AreEqual(2, cart.Count);
        }

        [TestMethod]
        public async Task CompleteCartCalculationUnitTest()
        {
            var cartService = _serviceProvider.GetService<CartService>();

            cartService.AddDishToCart(new BaseDish {Name = "Test1", Price = 90, BaseDishId = 1, BaseDishIngredients = new List<BaseDishIngredient>()});
            cartService.AddDishToCart(new BaseDish {Name = "Test2", Price = 100, BaseDishId = 2, BaseDishIngredients = new List<BaseDishIngredient>()});

            cartService.SetSessionIngredientsList(Constants.IngredientsSession, 
                new List<Ingredient>
                {
                    new Ingredient{AddExtraPrice = 5},
                    new Ingredient{AddExtraPrice = 10}
                });

            var cart = cartService.GetSessionCartList(Constants.CartSession);
            await cartService.SaveDishIngredients(cart.FirstOrDefault().Guid);

            var sumCart = cartService.GetSessionCartList(Constants.CartSession).Sum(od => od.Price);

            Assert.AreEqual(205, sumCart);
            Assert.AreEqual(2, cart.Count);
        }

        [TestMethod]
        public void CalculateCartDetailsIntergrationTest()
        {
            var cartService = _serviceProvider.GetService<CartService>();

            var orderedDishes = new List<OrderedDish>
            {
                new OrderedDish { Name = "Test1", Price = 100 , BaseDishId = 1 },
                new OrderedDish { Name = "Test2", Price = 110 , BaseDishId = 2 }
            };
            cartService.SetSessionCartList(Constants.CartSession, orderedDishes);
            var cartDetails = cartService.GetCartDetails();

            Assert.AreEqual(210, cartDetails.TotalPrice);
            Assert.AreEqual(2, cartDetails.Items);
            Assert.AreEqual(20, cartDetails.AddedIngredientsPrice);
        }
    }
}
