using HäggesPizzeria.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace HäggesPizzeria.Data
{
    public static class DbInitializer
    {
        public static void Initialize(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            var aUser = new ApplicationUser {UserName = "kund@test.com", Email = "kund@test.com"};
            var userResult = userManager.CreateAsync(aUser, "pass");

            var adminRole = new IdentityRole { Name = "Admin" };
            var roleResult = roleManager.CreateAsync(adminRole).Result;

            var adminUser = new ApplicationUser {UserName = "admin@test.com", Email = "admin@test.com"};
            var adminUserResult = userManager.CreateAsync(adminUser, "pass").Result;
            var adminRoleResult = userManager.AddToRoleAsync(adminUser, "Admin");

            if (context.BaseDishes.ToList().Count == 0)
            {
                var cheese = new Ingredient {Name = "Cheese", AddExtraPrice = 5};
                var tomatoe = new Ingredient {Name = "Tomatoe", AddExtraPrice = 5};
                var ham = new Ingredient {Name = "Ham", AddExtraPrice = 10};
                var mushroom = new Ingredient {Name = "Mushroom", AddExtraPrice = 10};
                var pineapple = new Ingredient {Name = "Pineapple", AddExtraPrice = 10};

                var capricciosa = new BaseDish {Name = "Capricciosa", Price = 100};
                var margaritha = new BaseDish {Name = "Margaritha", Price = 90};
                var hawaii = new BaseDish {Name = "Hawaii", Price = 100};

                var order1 = new Order {TotalPrice = 200, User = aUser};
                var order2 = new Order {TotalPrice = 100};

                var orderedDish1 = new OrderedDish {Name = "Capricciosa", Price = 100};
                var orderedDish1Cheese = new OrderedDishIngredient {OrderedDish = orderedDish1, Ingredient = cheese};
                var orderedDish1Tomatoe = new OrderedDishIngredient { OrderedDish = orderedDish1, Ingredient = tomatoe };
                var orderedDish1Ham = new OrderedDishIngredient { OrderedDish = orderedDish1, Ingredient = ham };
                var orderedDish1Mushroom = new OrderedDishIngredient { OrderedDish = orderedDish1, Ingredient = mushroom };
                orderedDish1.OrderedDishIngredients = new List<OrderedDishIngredient>
                {
                    orderedDish1Cheese,
                    orderedDish1Tomatoe,
                    orderedDish1Ham,
                    orderedDish1Mushroom
                };

                var orderedDish2 = new OrderedDish { Name = "Margaritha", Price = 90};
                var orderedDish2Cheese = new OrderedDishIngredient {OrderedDish = orderedDish2, Ingredient = cheese};
                var orderedDish2Tomatoe = new OrderedDishIngredient { OrderedDish = orderedDish2, Ingredient = tomatoe};
                var orderedDish2Mushroom = new OrderedDishIngredient {OrderedDish = orderedDish2, Ingredient = mushroom};
                orderedDish2.OrderedDishIngredients = new List<OrderedDishIngredient>
                {
                    orderedDish2Cheese,
                    orderedDish2Tomatoe,
                    orderedDish2Mushroom
                };

                var orderedDish3 = new OrderedDish { Name = "Hawaii", Price = 100};
                var orderedDish3Cheese = new OrderedDishIngredient {OrderedDish = orderedDish3, Ingredient = cheese};
                var orderedDish3Tomatoe = new OrderedDishIngredient { OrderedDish = orderedDish3, Ingredient = tomatoe };
                var orderedDish3Ham = new OrderedDishIngredient { OrderedDish = orderedDish3, Ingredient = ham };
                var orderedDish3Pineapple = new OrderedDishIngredient { OrderedDish = orderedDish3, Ingredient = pineapple };
                orderedDish3.OrderedDishIngredients = new List<OrderedDishIngredient>
                {
                    orderedDish3Cheese,
                    orderedDish3Tomatoe,
                    orderedDish3Ham,
                    orderedDish3Pineapple
                };

                var capricciosaCheese = new BaseDishIngredient {Dish = capricciosa, Ingredient = cheese};
                var capricciosaTomatoe = new BaseDishIngredient {Dish = capricciosa, Ingredient = tomatoe};
                var capricciosaHam = new BaseDishIngredient {Dish = capricciosa, Ingredient = ham};
                var capricciosaMushroom = new BaseDishIngredient {Dish = capricciosa, Ingredient = mushroom};
                capricciosa.BaseDishIngredients = new List<BaseDishIngredient>
                {
                    capricciosaCheese,
                    capricciosaHam,
                    capricciosaTomatoe,
                    capricciosaMushroom
                };

                var margarithaCheese = new BaseDishIngredient {Dish = margaritha, Ingredient = cheese};
                var margarithaHam = new BaseDishIngredient { Dish = margaritha, Ingredient = ham };
                margaritha.BaseDishIngredients = new List<BaseDishIngredient> {margarithaCheese, margarithaHam};

                var hawaiiCheese = new BaseDishIngredient {Dish = hawaii, Ingredient = cheese};
                var hawaiiHam = new BaseDishIngredient { Dish = hawaii, Ingredient = ham };
                var hawaiiPineapple = new BaseDishIngredient { Dish = hawaii, Ingredient = pineapple };
                hawaii.BaseDishIngredients = new List<BaseDishIngredient> {hawaiiCheese, hawaiiHam, hawaiiPineapple};

                order1.OrderedDishes = new List<OrderedDish> { orderedDish1, orderedDish2 };
                order2.OrderedDishes = new List<OrderedDish> { orderedDish3 };

                context.AddRange(cheese, tomatoe, ham, mushroom, pineapple);
                context.AddRange(capricciosa, margaritha, hawaii);
                context.AddRange(order1, order2);
                context.AddRange(orderedDish1, orderedDish2, orderedDish3);

                context.SaveChanges();
            }
        }
    }
}
