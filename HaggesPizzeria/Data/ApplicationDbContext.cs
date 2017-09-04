using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HaggesPizzeria.Models;

namespace HaggesPizzeria.Data
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
                .HasOne(bdi => bdi.BaseDish)
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

        public DbSet<BaseDish> BaseDishes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<BaseDishIngredient> BaseDishIngredients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderedDish> OrderedDishes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<OrderedDishIngredient> OrderedDishIngredients { get; set; }
    }
}
