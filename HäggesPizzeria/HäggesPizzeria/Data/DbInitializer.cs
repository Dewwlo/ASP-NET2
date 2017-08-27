using System;
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
                // Ingredients
                var cheese = new Ingredient {Name = "Cheese", AddExtraPrice = 5, IsActive = true};
                var tomatoe = new Ingredient {Name = "Tomatoe", AddExtraPrice = 5, IsActive = true };
                var ham = new Ingredient {Name = "Ham", AddExtraPrice = 10, IsActive = true };
                var mushroom = new Ingredient {Name = "Mushroom", AddExtraPrice = 10, IsActive = true };
                var pineapple = new Ingredient {Name = "Pineapple", AddExtraPrice = 10, IsActive = true };
            
                // Dishes
                var capricciosa = new BaseDish {Name = "Capricciosa", Price = 100, IsActive = true};
                var margaritha = new BaseDish {Name = "Margaritha", Price = 90, IsActive = true};
                var hawaii = new BaseDish {Name = "Hawaii", Price = 100, IsActive = true};

                // Orders
                var order1 = new Order {TotalPrice = 200, User = aUser, OrderDate = DateTime.Now};
                var order2 = new Order {TotalPrice = 100, OrderDate = DateTime.Now};

                var orderedDish1 = new OrderedDish {Name = "Capricciosa", Price = 100};
                orderedDish1.OrderedDishIngredients = new List<OrderedDishIngredient>
                {
                    new OrderedDishIngredient {OrderedDish = orderedDish1, Ingredient = cheese},
                    new OrderedDishIngredient {OrderedDish = orderedDish1, Ingredient = tomatoe},
                    new OrderedDishIngredient {OrderedDish = orderedDish1, Ingredient = ham},
                    new OrderedDishIngredient {OrderedDish = orderedDish1, Ingredient = mushroom}
                };

                var orderedDish2 = new OrderedDish {Name = "Margaritha", Price = 90};
                orderedDish2.OrderedDishIngredients = new List<OrderedDishIngredient>
                {
                    new OrderedDishIngredient {OrderedDish = orderedDish2, Ingredient = cheese},
                    new OrderedDishIngredient {OrderedDish = orderedDish2, Ingredient = tomatoe},
                    new OrderedDishIngredient {OrderedDish = orderedDish2, Ingredient = mushroom}
                };

                var orderedDish3 = new OrderedDish {Name = "Hawaii", Price = 100};
                orderedDish3.OrderedDishIngredients = new List<OrderedDishIngredient>
                {
                    new OrderedDishIngredient {OrderedDish = orderedDish3, Ingredient = cheese},
                    new OrderedDishIngredient {OrderedDish = orderedDish3, Ingredient = tomatoe},
                    new OrderedDishIngredient {OrderedDish = orderedDish3, Ingredient = ham},
                    new OrderedDishIngredient {OrderedDish = orderedDish3, Ingredient = pineapple}
                };

                capricciosa.BaseDishIngredients = new List<BaseDishIngredient>
                {
                    new BaseDishIngredient {BaseDish = capricciosa, Ingredient = cheese},
                    new BaseDishIngredient {BaseDish = capricciosa, Ingredient = tomatoe},
                    new BaseDishIngredient {BaseDish = capricciosa, Ingredient = ham},
                    new BaseDishIngredient {BaseDish = capricciosa, Ingredient = mushroom}
                };

                margaritha.BaseDishIngredients = new List<BaseDishIngredient>
                {
                    new BaseDishIngredient {BaseDish = margaritha, Ingredient = cheese},
                    new BaseDishIngredient {BaseDish = margaritha, Ingredient = ham}
                };

                hawaii.BaseDishIngredients = new List<BaseDishIngredient>
                {
                    new BaseDishIngredient {BaseDish = hawaii, Ingredient = cheese},
                    new BaseDishIngredient {BaseDish = hawaii, Ingredient = ham},
                    new BaseDishIngredient {BaseDish = hawaii, Ingredient = pineapple}
                };

                order1.OrderedDishes = new List<OrderedDish> {orderedDish1, orderedDish2};
                order2.OrderedDishes = new List<OrderedDish> {orderedDish3};

                context.AddRange(cheese, tomatoe, ham, mushroom, pineapple);
                context.AddRange(capricciosa, margaritha, hawaii);
                context.AddRange(order1, order2);
                context.AddRange(orderedDish1, orderedDish2, orderedDish3);

                context.SaveChanges();
            }
        }
    }
}
