using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
