using API.Interfaces;
using API.Models.Cart;
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

        public CartController(ICartService cartService,
                ICartHelper cartHelper)
        {
            _cartService = cartService;
            _cartHelper = cartHelper;
        }

        [HttpGet, Route("dishes")]
        public IActionResult GetDishes()
        {
            try
            {
                IEnumerable<CartDishesDTO> cartDishDTO = _cartService.GetCartDishes(GetCurrentUserId());

                return new ObjectResult(_cartHelper.ConvertCartDishesDTOsToCartDishesModel(cartDishDTO));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("fullprice")]
        public IActionResult GetFullPrice()
        {
            try
            {               
                string fullPrice = _cartService.FullPriceCart(GetCurrentUserId()).ToString();

                return new ObjectResult(fullPrice);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}")]
        public IActionResult Post(int id)
        {
            try
            {
                _cartService.AddDishToCart(id, GetCurrentUserId());

                return Ok(id);
            }
            catch (ValidationException ex)
            {
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

                return Ok();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _cartService.DeleteCartDish(id, GetCurrentUserId());

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route("all/delete")]
        public IActionResult DeleteAllDishesInCart()
        {
            try
            {
                _cartService.AllDeleteDishesToCart(GetCurrentUserId());

                return Ok();
            }
            catch (ValidationException ex)
            {
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
