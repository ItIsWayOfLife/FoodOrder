using Core.Constants;
using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Web.Models.Order;
using Web.Interfaces;

namespace Web.Controllers
{
    [Authorize(Roles = "employee")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "order";
        private readonly string _path;

        public OrderController(IOrderService orderService,
            ICartService cartService,
            ILoggerService loggerService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _loggerService = loggerService;
            _path = PathConstants.PATH_DISH;
        }

        [HttpPost]
        public IActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = GetCurrentUserId();

                try
                {
                    _orderService.Create(currentUserId);
                _cartService.AllDeleteDishesToCart(currentUserId);
                }
                catch (ValidationException ex)
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_CREATE, LoggerConstants.TYPE_POST, $"create order error: {ex.Message}", currentUserId);

                    return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
                }

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_CREATE, LoggerConstants.TYPE_POST, $"create order successful", currentUserId);

                return RedirectToAction("Index");
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Index(SortState sortOrder = SortState.DateOrderAsc)
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = GetCurrentUserId();

                IEnumerable<OrderDTO> orderDTOs = _orderService.GetOrders(currentUserId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDTO, OrderViewModel>()).CreateMapper();
                var orders = mapper.Map<IEnumerable<OrderDTO>, List<OrderViewModel>>(orderDTOs);

                ViewData["IdSort"] = sortOrder == SortState.IdAsc ? SortState.IdDesc : SortState.IdAsc;
                ViewData["DateSort"] = sortOrder == SortState.DateOrderAsc ? SortState.DateOrderDesc : SortState.DateOrderAsc;
                ViewData["FullPriceSort"] = sortOrder == SortState.FullPriceAsc ? SortState.FullPriceDesc : SortState.FullPriceAsc;
                ViewData["CountDishSort"] = sortOrder == SortState.CountDishAsc ? SortState.CountDishDesc : SortState.CountDishAsc;

                orders = sortOrder switch
                {
                    SortState.IdDesc => orders.OrderByDescending(s => s.Id).ToList(),
                    SortState.DateOrderAsc => orders.OrderBy(s => s.DateOrder).ToList(),
                    SortState.DateOrderDesc => orders.OrderByDescending(s => s.DateOrder).ToList(),
                    SortState.FullPriceAsc => orders.OrderBy(s => s.FullPrice).ToList(),
                    SortState.FullPriceDesc => orders.OrderByDescending(s => s.FullPrice).ToList(),
                    SortState.CountDishAsc => orders.OrderBy(s => s.CountDish).ToList(),
                    SortState.CountDishDesc => orders.OrderByDescending(s => s.CountDish).ToList(),
                    _ => orders.OrderBy(s => s.Id).ToList(),
                };

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX, LoggerConstants.TYPE_GET, "index", currentUserId);

                return View(orders);
            }

            _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX, LoggerConstants.TYPE_GET, LoggerConstants.ERROR_USER_NOT_AUTHENTICATED, null);

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult GetOrderDishes(int orderId)
        {
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = GetCurrentUserId();

                IEnumerable<OrderDishesDTO> orderDishesDTOs = _orderService.GetOrderDishes(currentUserId, orderId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDishesDTO, OrderDishesViewModel>()).CreateMapper();
                var orderDishes = mapper.Map<IEnumerable<OrderDishesDTO>, List<OrderDishesViewModel>>(orderDishesDTOs);

                foreach (var oD in orderDishes)
                {
                    oD.Path = _path + oD.Path;
                }

                ViewData["FullPrice"] = _orderService.GetOrders(currentUserId).Where(p => p.Id == orderId).FirstOrDefault().FullPrice;

                _loggerService.LogInformation(CONTROLLER_NAME + $"/getorderdishes/{orderId}", LoggerConstants.TYPE_GET, $"get order id: {orderId}", currentUserId);

                return View(orderDishes);
            }

            _loggerService.LogWarning(CONTROLLER_NAME + $"/getorderdishes/{orderId}", LoggerConstants.TYPE_GET, LoggerConstants.ERROR_USER_NOT_AUTHENTICATED, null);

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
