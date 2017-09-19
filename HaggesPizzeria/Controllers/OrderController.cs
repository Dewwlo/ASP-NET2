using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HaggesPizzeria.Models;
using HaggesPizzeria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace HaggesPizzeria.Controllers
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
        public async Task<IActionResult> OrdersInProgress()
        {
            return View(await _orderService.GetNonCompletedOrdersWithOrderedDishes());
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SetOrderComplete(int id)
        {
            _orderService.SetOrderComplete(id);
            return View("OrdersInProgress", await _orderService.GetNonCompletedOrdersWithOrderedDishes());
        }

        public async Task<IActionResult> ValidateShippingInformation()
        {
            var order = new Order();
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                order.Email = user.Email;
                order.Address = user.Address;
                order.Zipcode = user.Zipcode;
                order.PhoneNumber = user.PhoneNumber;
            }

            return View("ShippingInformation", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ValidateShippingInformation([Bind("Email,Address,PhoneNumber,Zipcode")] Order order)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString(Constants.OrderSession, JsonConvert.SerializeObject(order));
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
                var sessionOrder = HttpContext.Session.GetString(Constants.OrderSession);
                var sessionCart = HttpContext.Session.GetString(Constants.CartSession);

                if (sessionCart != null
                    && sessionOrder != null
                    && JsonConvert.DeserializeObject<List<OrderedDish>>(sessionCart).Count >= 1)
                {
                    _orderService.SaveOrder(JsonConvert.DeserializeObject<Order>(sessionOrder));
                    _orderService.SaveOrderedDishes(JsonConvert.DeserializeObject<List<OrderedDish>>(sessionCart));
                    HttpContext.Session.Remove(Constants.CartSession);
                    HttpContext.Session.Remove(Constants.OrderSession);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("Error", "Something went wrong, make sure you have items in your cart.");

                return View("Payment");
            }
            ModelState.AddModelError("Error", "Not a valid credit card");

            return View("Payment");
        }
    }
}
