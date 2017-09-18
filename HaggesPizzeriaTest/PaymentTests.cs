using System;
using System.Threading.Tasks;
using HaggesPizzeria.Models;
using HaggesPizzeria.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HaggesPizzeriaTest
{
    [TestClass]
    public class PaymentTests : BaseTests
    {

        [TestMethod]
        public void PaymentValidationUnitTest()
        {
            var paymentService = _serviceProvider.GetService<PaymentService>();

            var valid = paymentService.ValidatePaymentInformation(new Payment
            {
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                CardNumber = "2222222222222222",
                Cvc = "234"
            });

            var nonValid = paymentService.ValidatePaymentInformation(new Payment
            {
                Year = 2016,
                Month = 9,
                CardNumber = "2222222222222222",
                Cvc = "234"
            });

            Assert.IsTrue(valid);
            Assert.IsFalse(nonValid);
        }
    }
}
