﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HäggesPizzeria.Models;
using HäggesPizzeria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace HäggesPizzeria.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PaymentService _paymentService;
        private readonly OrderService _orderService;

        public OrderController(UserManager<ApplicationUser> userManager, PaymentService paymentService, OrderService orderService)
        {
            _userManager = userManager;
            _paymentService = paymentService;
            _orderService = orderService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _orderService.GetAllOrdersWithOrderedDishes());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Details(int id)
        {
            var order = _orderService.GetOrderWithOrderedDishes(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> ValidateShippingInformation()
        {
            var order = new Order();
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                order.Email = user.Email;
                order.Adress = user.Adress;
                order.Zipcode = user.Zipcode;
                order.PhoneNumber = user.PhoneNumber;
            }

            return View("ShippingInformation", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ValidateShippingInformation([Bind("Email,Adress,PhoneNumber,Zipcode")] Order order)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("OrderInformation", JsonConvert.SerializeObject(order));
                return RedirectToAction("ValidatePayment");
            }

            return View("ShippingInformation", order);
        }

        public IActionResult ValidatePayment()
        {
            return View("Payment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ValidatePayment(Payment payment)
        {
            if (ModelState.IsValid && _paymentService.ValidatePaymentInformation(payment))
            {
                var sessionOrder = HttpContext.Session.GetString("OrderInformation");
                var sessionCart = HttpContext.Session.GetString("Cart");

                if (sessionCart != null && sessionOrder != null)
                {
                    _orderService.SaveOrder(JsonConvert.DeserializeObject<Order>(sessionOrder));
                    _orderService.SaveOrderedDishes(JsonConvert.DeserializeObject<List<OrderedDish>>(sessionCart));
                    HttpContext.Session.Remove("Cart");
                    HttpContext.Session.Remove("OrderInformation");
                }

                return RedirectToAction("Index", "Home");
            }

            return View("Payment");
        }
    }
}
