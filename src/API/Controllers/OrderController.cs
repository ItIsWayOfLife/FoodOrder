using API.Interfaces;
using API.Models.Order;
using AutoMapper;
using Core.Constants;
using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin, employee")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IOrderHelper _orderHelper;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "api/menu";

        public OrderController(IOrderService orderService,
            ICartService cartService,
            IOrderHelper orderHelper,
            ILoggerService loggerService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _orderHelper = orderHelper;
            _loggerService = loggerService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var orderDTOs = _orderService.GetOrders(GetCurrentUserId()).OrderByDescending(p => p.DateOrder).ToList();
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDTO, OrderModel>()).CreateMapper();
                var orders = mapper.Map<IEnumerable<OrderDTO>, List<OrderModel>>(orderDTOs);

                for (int i = 0; i > orderDTOs.Count(); i++)
                {
                    orders[i].DateOrder = orderDTOs[i].DateOrder.ToString();
                }

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_GET, $"get orders successful", GetCurrentUserId());

                return new ObjectResult(orders);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_GET, $"get orders error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetDishes(int id)
        {
            try
            {
                IEnumerable<OrderDishesDTO> orderDishesDTOs = _orderService.GetOrderDishes(GetCurrentUserId(), id);

                _loggerService.LogInformation(CONTROLLER_NAME +$"/{id}", LoggerConstants.TYPE_GET, $"get order dishes order: {id} successful", GetCurrentUserId());

                return new ObjectResult(_orderHelper.ConvertOrderDishesDTOToOrderDishesModels(orderDishesDTOs));
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/{id}", LoggerConstants.TYPE_GET, $"get order dishes order: {id} successful", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                // create order
                _orderService.Create(GetCurrentUserId());
                // emptying the cart
                _cartService.AllDeleteDishesToCart(GetCurrentUserId());

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"create order successful", GetCurrentUserId());

                return Ok();
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"create order error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
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
