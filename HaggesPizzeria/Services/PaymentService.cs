using System;
using HaggesPizzeria.Models;

namespace HaggesPizzeria.Services
{
    public class PaymentService
    {
        public bool ValidatePaymentInformation(Payment payment)
        {
            return (payment.Year >= DateTime.Now.Year && payment.Month >= DateTime.Now.Month)
                       && (payment.CardNumber.Length == 16)
                       && (payment.Cvc.Length == 3);
        }
    }
}
