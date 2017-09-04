using HäggesPizzeria.Models;
using HäggesPizzeria.Services;
using Microsoft.AspNetCore.Mvc;

namespace HäggesPizzeria.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly CartService _cartService;

        public CartViewComponent(CartService cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke(CartDetails cartDetails)
        {
            return View(_cartService.GetCartDetails(HttpContext));
        }
    }
}
