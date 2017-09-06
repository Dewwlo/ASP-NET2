using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaggesPizzeria.Models;
using HaggesPizzeria.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HaggesPizzeriaTest
{
    [TestClass]
    public class BaseDishServiceTest : BaseTests
    {
        public override void InitializeDatabase()
        {
            base.InitializeDatabase();
        }

        [TestMethod]
        public async Task TestBaseDishesList()
        {
            var testBaseDishes = await _serviceProvider.GetService<BaseDishService>().GetAllBaseDishes();
            var mockBaseDishes = Mock.Of<List<BaseDish>>();

            CollectionAssert.AreEqual(mockBaseDishes, testBaseDishes.ToList());
        }
    }
}
