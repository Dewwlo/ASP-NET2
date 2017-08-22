﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HäggesPizzeria.Models;

namespace HäggesPizzeria.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BaseDishIngredient>()
                .HasKey(bdi => new { bdi.BaseDishId, bdi.IngredientId });

            builder.Entity<BaseDishIngredient>()
                .HasOne(bdi => bdi.Dish)
                .WithMany(bd => bd.BaseDishIngredients)
                .HasForeignKey(bdi => bdi.BaseDishId);

            builder.Entity<BaseDishIngredient>()
                .HasOne(bdi => bdi.Ingredient)
                .WithMany(i => i.BaseDishIngredients)
                .HasForeignKey(bdi => bdi.IngredientId);

            builder.Entity<OrderedDishIngredient>()
                .HasKey(odi => new {odi.OrderedDishId, odi.IngredientId});

            builder.Entity<OrderedDishIngredient>()
                .HasOne(odi => odi.OrderedDish)
                .WithMany(od => od.OrderedDishIngredients)
                .HasForeignKey(odi => odi.OrderedDishId);

            builder.Entity<OrderedDishIngredient>()
                .HasOne(odi => odi.Ingredient)
                .WithMany(i => i.OrderedDishIngredients)
                .HasForeignKey(odi => odi.IngredientId);
                

            base.OnModelCreating(builder);
            
        }

        public DbSet<BaseDish> Dishes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<BaseDishIngredient> DishIngredients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderedDish> OrderedDishes { get; set; }
        public DbSet<OrderedDishIngredient> OrderedDishIngredients { get; set; }
    }
}
