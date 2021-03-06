﻿using System;
using System.Threading.Tasks;
using HaggesPizzeria.Models;
using HaggesPizzeria.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HaggesPizzeria.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> AddDishToCart(int dishId)
        {
            await _cartService.AddDishToCart(dishId);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RemoveDishFromCart(Guid guid)
        {
            _cartService.RemoveDishFromCart(guid);

            if (_cartService.CartHasItems())
            {
                return View("Cart", _cartService.GetSessionCartList(Constants.CartSession));
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult GetAllCartItems()
        {
            var sessionCart = HttpContext.Session.GetString(Constants.CartSession);

            if (sessionCart != null)
            {
                return View("Cart", _cartService.GetSessionCartList(Constants.CartSession));
            }

            return View("Cart");
        }

        public IActionResult DishDetails(Guid guid)
        {
            return View("CartDishDetails", _cartService.GetDishDetails(HttpContext.Session, guid));
        }

        public async Task<IActionResult> SaveDishIngredients(Guid guid)
        {
            return View("Cart", await _cartService.SaveDishIngredients(guid));
        }
    }
}