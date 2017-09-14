using System;
using HaggesPizzeria.Data;
using HaggesPizzeria.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace HaggesPizzeriaTest
{
    public class BaseTests
    {
        public IServiceProvider _serviceProvider;

        public BaseTests()
        {
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            var services = new ServiceCollection();
            services
                .AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>()
                .AddDbContext<ApplicationDbContext>(b => b.UseInMemoryDatabase("Scratch").UseInternalServiceProvider(efServiceProvider))
                .AddTransient<IEmailSender, EmailSender>()
                .AddTransient<BaseDishService>()
                .AddTransient<CartService>()
                .AddTransient<CategoryService>()
                .AddTransient<IngredientService>()
                .AddTransient<OrderService>()
                .AddTransient<PaymentService>()
                .AddSingleton(typeof(ISession), new TestSession())
                .AddSession()
                .AddMvc();

            _serviceProvider = services.BuildServiceProvider();
            services.AddSingleton<IServiceProvider>(_serviceProvider);

            InitializeDatabase();
        }

        public virtual void InitializeDatabase()
        {
            
        }
    }
}
