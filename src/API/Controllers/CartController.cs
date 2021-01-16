using API.Interfaces;
using API.Models.Cart;
using Core.Constants;
using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin, employee")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICartHelper _cartHelper;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "api/cart";

        public CartController(ICartService cartService,
                ICartHelper cartHelper,
                ILoggerService loggerService)
        {
            _cartService = cartService;
            _cartHelper = cartHelper;
            _loggerService = loggerService;
        }

        [HttpGet, Route("dishes")]
        public IActionResult GetDishes()
        {
            try
            {
                IEnumerable<CartDishesDTO> cartDishDTO = _cartService.GetCartDishes(GetCurrentUserId());

                _loggerService.LogInformation(CONTROLLER_NAME + "/dishes", LoggerConstants.TYPE_GET, "get cart dishes successful", GetCurrentUserId());

                return new ObjectResult(_cartHelper.ConvertCartDishesDTOsToCartDishesModel(cartDishDTO));
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + "/dishes", LoggerConstants.TYPE_GET, $"get cart dishes error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("fullprice")]
        public IActionResult GetFullPrice()
        {
            try
            {               
                string fullPrice = _cartService.FullPriceCart(GetCurrentUserId()).ToString();

                _loggerService.LogInformation(CONTROLLER_NAME + "/fullprice", LoggerConstants.TYPE_GET, "get full price successful", GetCurrentUserId());

                return new ObjectResult(fullPrice);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + "/fullprice", LoggerConstants.TYPE_GET, $"get full price error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}")]
        public IActionResult Post(int id)
        {
            try
            {
                _cartService.AddDishToCart(id, GetCurrentUserId());

                _loggerService.LogInformation(CONTROLLER_NAME + $"/{id}", LoggerConstants.TYPE_POST, $"add dish id: {id} to cart successful", GetCurrentUserId());

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/{id}", LoggerConstants.TYPE_POST, $"add dish id: {id} to cart error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateCartDishes(List<CartDishesUpdateModel> models)
        {
            if (models == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                foreach (var m in models)
                {
                    _cartService.UpdateCountDishInCart(GetCurrentUserId(), m.Id, m.Count);
                }

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"update count dish in cart successful", GetCurrentUserId());

                return Ok();
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"update count dish in cart error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _cartService.DeleteCartDish(id, GetCurrentUserId());

                _loggerService.LogInformation(CONTROLLER_NAME +$"/{id}", LoggerConstants.ACTION_DELETE, $"delete cart dish id: {id} successful", GetCurrentUserId());

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/{id}", LoggerConstants.ACTION_DELETE, $"delete cart dish id: {id} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route("all/delete")]
        public IActionResult DeleteAllDishesInCart()
        {
            try
            {
                _cartService.AllDeleteDishesToCart(GetCurrentUserId());

                _loggerService.LogInformation(CONTROLLER_NAME + $"/all/delete", LoggerConstants.ACTION_DELETE, $"delete all dish in cart successful", GetCurrentUserId());

                return Ok();
            }
            catch (ValidationException ex)
            {
                _loggerService.LogInformation(CONTROLLER_NAME + $"/all/delete", LoggerConstants.ACTION_DELETE, $"delete all dish in cart error: {ex.Message}", GetCurrentUserId());

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
