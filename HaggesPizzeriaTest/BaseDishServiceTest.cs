using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaggesPizzeria.Models;
using HaggesPizzeria.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HaggesPizzeriaTest
{
    [TestClass]
    public class BaseDishServiceTest : BaseTests
    {
        [TestMethod]
        public async Task FilterDishesUnitTest()
        {
            var ingredient1 = new Ingredient { IngredientId = 1, IsActive = true };
            var ingredient2 = new Ingredient { IngredientId = 2, IsActive = false };

            var baseDishes = new List<BaseDish>
            {
                new BaseDish
                {
                    BaseDishId = 1,
                    BaseDishIngredients = new List<BaseDishIngredient>
                    {
                        new BaseDishIngredient { BaseDishId = 1, IngredientId = 1, Ingredient = ingredient1 }
                    }
                },
                new BaseDish
                {
                    BaseDishId = 2,
                    BaseDishIngredients = new List<BaseDishIngredient>
                    {
                        new BaseDishIngredient { BaseDishId = 1, IngredientId = 2, Ingredient = ingredient2 }
                    }
                }
            };

            var filteredBaseDishes = _serviceProvider.GetService<BaseDishService>().FilterOutDishesWithInactiveIngredients(baseDishes);

            Assert.AreEqual(1, filteredBaseDishes.Count);
        }
    }
}
