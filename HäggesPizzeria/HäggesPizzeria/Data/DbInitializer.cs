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
            var aUser = new ApplicationUser();
            aUser.UserName = "kund@test.com";
            aUser.Email = "kund@test.com";
            var r = userManager.CreateAsync(aUser, "PassWord1$").Result;

            var adminRole = new IdentityRole { Name = "Admin" };
            var roleResult = roleManager.CreateAsync(adminRole).Result;

            var adminUser = new ApplicationUser();
            adminUser.UserName = "admin@test.com";
            adminUser.Email = "admin@test.com";
            var adminUserResult = userManager.CreateAsync(adminUser, "Password1$").Result;

            userManager.AddToRoleAsync(adminUser, "Admin");

            if (context.Dishes.ToList().Count == 0)
            {
                var cheese = new Ingredient { Name = "Cheese" };
                var tomatoe = new Ingredient { Name = "Tomatoe" };
                var ham = new Ingredient { Name = "Ham" };

                var capricciosa = new Dish { Name = "Capricciosa" };
                //var margaritha = new Dish { Name = "Margaritha" };
                //var hawaii = new Dish { Name = "Hawaii" };

                context.AddRange(tomatoe, cheese, ham);
                //context.AddRange(capricciosa, margaritha, hawaii);
                context.AddRange(capricciosa);

                var capricciosaCheese = new DishIngredient { Dish = capricciosa, Ingredient = cheese };
                var capricciosaTomatoe = new DishIngredient { Dish = capricciosa, Ingredient = tomatoe };
                var capricciosaHam = new DishIngredient { Dish = capricciosa, Ingredient = ham };

                capricciosa.DishIngredients = new List<DishIngredient>();
                //margaritha.DishIngredients = new List<DishIngredient>();
                //hawaii.DishIngredients = new List<DishIngredient>();
                capricciosa.DishIngredients.Add(capricciosaCheese);
                capricciosa.DishIngredients.Add(capricciosaHam);
                capricciosa.DishIngredients.Add(capricciosaTomatoe);
                
                context.SaveChanges();
            }
        }
    }
}
