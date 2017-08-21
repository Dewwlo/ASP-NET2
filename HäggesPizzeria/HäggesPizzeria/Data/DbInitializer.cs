using HäggesPizzeria.Models;
using HäggesPizzeria.Models.Dish;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                var capricciosa = new Dish { Name = "Capricciosa" };
                var margaritha = new Dish { Name = "Margaritha" };
                var hawaii = new Dish { Name = "Hawaii" };
                context.AddRange(capricciosa, margaritha, hawaii);
                context.SaveChanges();
            }
        }
    }
}
