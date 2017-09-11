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
            context.BaseDishes.Add(new BaseDish { Name = "Test2", Price = 100 });
            context.BaseDishIngredients.Add(new BaseDishIngredient {IngredientId = 3, BaseDishId = 1});
            context.SaveChanges();
        }
        
        [TestMethod]
        public async Task TestCompleteCartCalculation()
        {
            var cartService = _serviceProvider.GetService<CartService>();
            var ingredientService = _serviceProvider.GetService<IngredientService>();

            await cartService.AddDishToCart(1);
            await cartService.AddDishToCart(2);
            cartService.SetSessionIngredientsList(Constants.IngredientsSession, new List<Ingredient>());

            //TODO Fix: Session variables set will only be active in the sevice in which they are initiated.

            //var ingredient1 = ingredientService.AddIngredientToList(1);
            //cartService.SetSessionIngredientsList(Constants.IngredientsSession, ingredient1);
            //var ingredient2 = ingredientService.AddIngredientToList(2);

            cartService.SetSessionIngredientsList(
                Constants.IngredientsSession, 
                ingredientService.GetAllIngredients().Take(2).ToList());
            var cart = cartService.GetSessionCartList(Constants.CartSession);
            await cartService.SaveDishIngredients(cart.FirstOrDefault().Guid);

            var sumCart = cartService.GetSessionCartList(Constants.CartSession).Sum(od => od.Price);

            Assert.AreEqual(205, sumCart);
            Assert.AreEqual(2, cart.Count);
        }
    }
}
