using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaggesPizzeria.Data;
using HaggesPizzeria.Models;
using HaggesPizzeria.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            context.BaseDishIngredients.Add(new BaseDishIngredient {IngredientId = 3, BaseDishId = 1});
            context.SaveChanges();
        }
        
        [TestMethod]
        public async Task TestCompleteCartCalculation()
        {
            var httpContext = new DefaultHttpContext {Session = new TestSession()};

            var cartService = _serviceProvider.GetService<CartService>();
            var ingredientService = _serviceProvider.GetService<IngredientService>();

            await cartService.AddDishToCart(httpContext, 1);
            cartService.SetSessionIngredientsList(httpContext, "IngredientsList", new List<Ingredient>());

            var ingredient1 = ingredientService.AddIngredientToList(httpContext, 1);
            cartService.SetSessionIngredientsList(httpContext, "IngredientsList", ingredient1);
            var ingredient2 =  ingredientService.AddIngredientToList(httpContext, 2);
            cartService.SetSessionIngredientsList(httpContext, "IngredientsList", ingredient2);

            var cart = cartService.GetSessionCartList(httpContext, "Cart");
            await cartService.SaveDishIngredients(httpContext, cart.FirstOrDefault().Guid);

            var sumCart = cartService.GetSessionCartList(httpContext, "Cart").Sum(od => od.Price);

            Assert.AreEqual(105, sumCart);
        }
    }
}
