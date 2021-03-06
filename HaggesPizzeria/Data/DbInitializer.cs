﻿using System;
using HaggesPizzeria.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HaggesPizzeria.Data
{
    public static class DbInitializer
    {
        public static void Initialize(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Users & Roles
            var adminRole = new IdentityRole { Name = "Admin" };
            var roleResult = roleManager.CreateAsync(adminRole).Result;

            var aUser = new ApplicationUser
            {
                UserName = "kund@test.com",
                Email = "kund@test.com",
                Address = "sjövägen 3",
                Zipcode = "11320",
                CustomerName = "Kunden",
                PhoneNumber = "0720302020"
            };

            var adminUser = new ApplicationUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                Address = "sjövägen 3",
                Zipcode = "11320",
                CustomerName = "Admin",
                PhoneNumber = "0720302020"
            };

            var userResult = userManager.CreateAsync(aUser, "pass").Result;
            var adminUserResult = userManager.CreateAsync(adminUser, "pass").Result;
            var adminRoleResult = userManager.AddToRoleAsync(adminUser, "Admin").Result;
            
            // Ingredients
            var cheese = new Ingredient {Name = "Cheese", AddExtraPrice = 5, IsActive = true};
            var tomatoe = new Ingredient {Name = "Tomatoe", AddExtraPrice = 5, IsActive = true};
            var ham = new Ingredient {Name = "Ham", AddExtraPrice = 10, IsActive = true};
            var mushroom = new Ingredient {Name = "Mushroom", AddExtraPrice = 10, IsActive = true};
            var pineapple = new Ingredient {Name = "Pineapple", AddExtraPrice = 10, IsActive = true};

            // Categories
            var pizza = new Category { Name = "Pizza", IsActive = true};
            var pasta = new Category { Name = "Pasta", IsActive = true};

            // Dishes
            var capricciosa = new BaseDish {Name = "Capricciosa", Price = 100, IsActive = true, Category = pizza};
            var margherita = new BaseDish {Name = "Margherita", Price = 90, IsActive = true, Category = pizza};
            var hawaii = new BaseDish {Name = "Hawaii", Price = 100, IsActive = true, Category = pizza};
            var pastacarbonara = new BaseDish {Name = "Pasta carbonara", Price = 90, IsActive = true, Category = pasta};

            // Orders
            var order1 = new Order
            {
                TotalPrice = 200,
                User = aUser,
                OrderDate = DateTime.Now,
                Address = "Testvägen 3",
                Email = "Test@gmail.com",
                PhoneNumber = "23920323223",
                Zipcode = "12321",
                IsComplete = true
            };
            var order2 = new Order
            {
                TotalPrice = 100,
                OrderDate = DateTime.Now,
                Address = "Testvägen 3",
                Email = "Test@gmail.com",
                PhoneNumber = "23920323223",
                Zipcode = "12321",
                IsComplete = false
            };

            var orderedDish1 = new OrderedDish {Name = "Capricciosa", Price = 100, Category = pizza};
            orderedDish1.OrderedDishIngredients = new List<OrderedDishIngredient>
            {
                new OrderedDishIngredient {OrderedDish = orderedDish1, Ingredient = cheese},
                new OrderedDishIngredient {OrderedDish = orderedDish1, Ingredient = tomatoe},
                new OrderedDishIngredient {OrderedDish = orderedDish1, Ingredient = ham},
                new OrderedDishIngredient {OrderedDish = orderedDish1, Ingredient = mushroom}
            };

            var orderedDish2 = new OrderedDish {Name = "Margaritha", Price = 90, Category = pizza};
            orderedDish2.OrderedDishIngredients = new List<OrderedDishIngredient>
            {
                new OrderedDishIngredient {OrderedDish = orderedDish2, Ingredient = cheese},
                new OrderedDishIngredient {OrderedDish = orderedDish2, Ingredient = tomatoe},
                new OrderedDishIngredient {OrderedDish = orderedDish2, Ingredient = mushroom}
            };

            var orderedDish3 = new OrderedDish {Name = "Hawaii", Price = 100, Category = pizza};
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

            margherita.BaseDishIngredients = new List<BaseDishIngredient>
            {
                new BaseDishIngredient {BaseDish = margherita, Ingredient = cheese},
                new BaseDishIngredient {BaseDish = margherita, Ingredient = ham}
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
            context.AddRange(capricciosa, margherita, hawaii, pastacarbonara);
            context.AddRange(order1, order2);
            context.AddRange(orderedDish1, orderedDish2, orderedDish3);
            context.AddRange(pizza, pasta);

            context.SaveChanges();
        }
    }
}
