using Core.Constants;
using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Web.Models.Cart;

namespace Web.Controllers
{
    [Authorize(Roles = "employee")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        private readonly string _path;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
            _path = PathConstants.PATH_DISH;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = GetCurrentUserId();
                CartDTO cartDTO = _cartService.GetCart(currentUserId);

                IEnumerable<CartDishesDTO> cartDishDTO = _cartService.GetCartDishes(currentUserId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<CartDishesDTO, CartDishesViewModel>()).CreateMapper();
                var cartDishes = mapper.Map<IEnumerable<CartDishesDTO>, List<CartDishesViewModel>>(cartDishDTO);

                foreach (var cD in cartDishes)
                {
                    cD.Path = _path + cD.Path;
                }

                ViewData["FullPrice"] = _cartService.FullPriceCart(currentUserId);

                return View(cartDishes);
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult Delete(int cartDishId)
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = GetCurrentUserId();

                try
                {
                    _cartService.DeleteCartDish(cartDishId, currentUserId);

                    return RedirectToAction("Index");
                }
                catch (ValidationException ex)
                {
                    return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
                }
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Add(int dishId)
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = GetCurrentUserId();

                _cartService.AddDishToCart(dishId, currentUserId);

                return RedirectToAction("Index");
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult DeleteAll()
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = GetCurrentUserId();

                try
                {
                    _cartService.AllDeleteDishesToCart(currentUserId);
                }
                catch (ValidationException ex)
                {
                    return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult Update(int dishCartId, int count)
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = GetCurrentUserId();

                try
                {
                    _cartService.UpdateCountDishInCart(currentUserId, dishCartId, count);
                }
                catch (ValidationException ex)
                {
                    return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("Login", "Account");
        }

        private string GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                return User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            else
            {
                return null;
            }
        }
    }
}
