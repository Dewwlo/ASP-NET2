using System;
using HaggesPizzeria.Models;

namespace HaggesPizzeria.Services
{
    public class PaymentService
    {
        public bool ValidatePaymentInformation(Payment payment)
        {
            // TODO Fix: Can not purchase if card expires same month.
            return (new DateTime(payment.Year, payment.Month, 1) >= DateTime.Now)
                   && (payment.Year >= DateTime.Now.Year)
                   && (payment.CardNumber.Length == 16)
                   && (payment.Cvc.Length == 3);
        }
    }
}
