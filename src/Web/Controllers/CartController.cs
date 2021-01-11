using Core.Constants;
using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Web.Models.Cart;
using Web.Interfaces;

namespace Web.Controllers
{
    [Authorize(Roles = "employee")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "cart";

        private readonly string _path;

        public CartController(ICartService cartService,
            ILoggerService loggerService)
        {
            _cartService = cartService;
            _loggerService = loggerService;
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

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX, LoggerConstants.TYPE_GET, "index", currentUserId);

                return View(cartDishes);
            }

            _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX, LoggerConstants.TYPE_GET, LoggerConstants.ERROR_USER_NOT_AUTHENTICATED, null);

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
                }
                catch (ValidationException ex)
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE + $"/{cartDishId}", LoggerConstants.TYPE_POST, $"delete cartDishId: {cartDishId} error: {ex.Message}", currentUserId);

                    return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
                }

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE + $"/{cartDishId}", LoggerConstants.TYPE_POST, $"delete cartDishId: {cartDishId} successful", currentUserId);

                return RedirectToAction("Index");
            }

            _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX + $"/{cartDishId}", LoggerConstants.TYPE_GET, LoggerConstants.ERROR_USER_NOT_AUTHENTICATED, null);

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Add(int dishId)
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = GetCurrentUserId();

                try
                {
                    _cartService.AddDishToCart(dishId, currentUserId);
                }
                catch (ValidationException ex)
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_ADD + $"{dishId}", LoggerConstants.TYPE_GET, $"add dish id: {dishId} to cart error: {ex.Message}", currentUserId);

                    return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
                }

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_ADD + $"{dishId}", LoggerConstants.TYPE_GET, $"add dish id: {dishId} to cart", currentUserId);

                return RedirectToAction("Index");
            }

            _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_ADD +$"/{dishId}", LoggerConstants.TYPE_GET, LoggerConstants.ERROR_USER_NOT_AUTHENTICATED, null);

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
                    _loggerService.LogWarning(CONTROLLER_NAME + $"/deleteall", LoggerConstants.TYPE_POST, $"delete all dihes in cart error: {ex.Message}", GetCurrentUserId());

                    return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
                }

                _loggerService.LogInformation(CONTROLLER_NAME + $"/deleteall", LoggerConstants.TYPE_POST, "delete all dihes in cart successful", GetCurrentUserId());

                return RedirectToAction("Index");
            }

            _loggerService.LogWarning(CONTROLLER_NAME + $"/deleteall", LoggerConstants.TYPE_POST, LoggerConstants.ERROR_USER_NOT_AUTHENTICATED, null);

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
                    _loggerService.LogWarning(CONTROLLER_NAME + $"/{dishCartId}&{count}", LoggerConstants.TYPE_POST, $"update dish id: {dishCartId} on count: {count} error: {ex.Message}", currentUserId);

                    return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
                }

                _loggerService.LogInformation(CONTROLLER_NAME + $"/{dishCartId}&{count}", LoggerConstants.TYPE_POST, $"update dish id: {dishCartId} on count: {count} successful", currentUserId);

                return RedirectToAction("Index");
            }

            _loggerService.LogWarning(CONTROLLER_NAME + $"/{dishCartId}&{count}", LoggerConstants.TYPE_POST, LoggerConstants.ERROR_USER_NOT_AUTHENTICATED, null);

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
