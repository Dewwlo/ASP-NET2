using System;
using HaggesPizzeria.Models;

namespace HaggesPizzeria.Services
{
    public class PaymentService
    {
        public bool ValidatePaymentInformation(Payment payment)
        {
            var verifyMonth = true;

            if (DateTime.Now.Year == payment.Year)
            {
                verifyMonth = payment.Month >= DateTime.Now.Month;
            }

            return (payment.Year >= DateTime.Now.Year && verifyMonth)
                       && (payment.CardNumber.Length == 16)
                       && (payment.Cvc.Length == 3);
        }
    }
}
