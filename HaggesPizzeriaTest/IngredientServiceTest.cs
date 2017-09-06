using System.Collections.Generic;
using System.Linq;
using HaggesPizzeria.Data;
using HaggesPizzeria.Models;
using HaggesPizzeria.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HaggesPizzeriaTest
{
    [TestClass]
    public class IngredientServiceTest : BaseTests
    {
        public override void InitializeDatabase()
        {
            base.InitializeDatabase();
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            context.Ingredients.Add(new Ingredient {Name = "AAA", IsActive = true});
            context.Ingredients.Add(new Ingredient {Name = "BBB", IsActive = true});
            context.Ingredients.Add(new Ingredient {Name = "CCC", IsActive = true});

            context.BaseDishes.Add(new BaseDish {Name = "Test"});
            context.BaseDishIngredients.AddRange(
                new BaseDishIngredient {BaseDishId = 1, IngredientId = 1}, 
                new BaseDishIngredient {BaseDishId = 1, IngredientId = 2}
                );

            context.SaveChanges();
        }

        [TestMethod]
        public void OnlyUnusedActiveIngredients()
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var ingredientList = new List<Ingredient> {context.Ingredients.FirstOrDefault()};
            var ingredients = _serviceProvider.GetService<IngredientService>()
                    .GetAllUnusedIngredients(ingredientList);
            Assert.AreEqual(2, ingredients.Count);
        }

        [TestMethod]
        public void OnlyUsedActiveIngredients()
        {
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            var ingredientList = new List<Ingredient> { context.Ingredients.FirstOrDefault() };
            var ingredients = _serviceProvider.GetService<IngredientService>()
                .GetAllUsedIngredients(context.BaseDishIngredients.Where(bdi => bdi.BaseDishId == 1).ToList());
            Assert.AreEqual(2, ingredients.Count);
        }
    }
}
