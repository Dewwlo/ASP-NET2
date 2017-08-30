using System;
using HäggesPizzeria.Models;

namespace HäggesPizzeria.Services
{
    public class PaymentService
    {
        public bool ValidatePaymentInformation(Payment payment)
        {
            return (new DateTime(payment.Year, payment.Month, 31) >= DateTime.Now)
                   && (payment.Year >= DateTime.Now.Year)
                   && (payment.CardNumber.Length == 16)
                   && (payment.Cvc.Length == 3);
        }
    }
}
