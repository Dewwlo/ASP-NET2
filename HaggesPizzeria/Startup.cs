using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HaggesPizzeria.Data;
using HaggesPizzeria.Models;
using HaggesPizzeria.Services;

namespace HaggesPizzeria
{
    public class Startup
    {
        public string Environment { get; set; }
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Environment = environment.EnvironmentName;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment == Constants.DevelopmentEnvironment)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("DefaultConnection"));
            }

            if (Environment == Constants.ProductionEnvironment)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            }

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 3;
            });

            // Add application services.
            services.AddTransient<UserManager<ApplicationUser>>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<BaseDishService>();
            services.AddTransient<CartService>();
            services.AddTransient<CategoryService>();
            services.AddTransient<IngredientService>();
            services.AddTransient<OrderService>();
            services.AddTransient<PaymentService>();

            services.AddMvc();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            IHostingEnvironment environment)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            if (environment.EnvironmentName == Constants.ProductionEnvironment)
            {
                context.Database.Migrate();
            }

            if (!context.Users.Any())
            {
                DbInitializer.Initialize(context, userManager, roleManager);
            }
        }
    }
}
