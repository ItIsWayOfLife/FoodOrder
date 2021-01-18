using API.Controllers;
using API.Interfaces;
using API.Models.Cart;
using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace API.UnitTests.Controllers
{
    public class CartControllerUnitTests
    {
        private readonly Mock<ICartService> _mockCartService;
        private readonly Mock<ICartHelper> _mockCartHelper;
        private readonly Mock<ILoggerService> _mockLoggerService;

        private readonly CartController _controller;

        public CartControllerUnitTests()
        {
            _mockCartService = new Mock<ICartService>();
            _mockCartHelper = new Mock<ICartHelper>();
            _mockLoggerService = new Mock<ILoggerService>();

            _controller = new CartController(
                _mockCartService.Object,
                _mockCartHelper.Object,
                _mockLoggerService.Object);
        }


        #region get

        [Fact]
        public void GetDishes_WhenCalled_ReturnsObjectResult()
        {
            var result = _controller.GetDishes();

            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void GetFullPrice_WhenCalled_ReturnsObjectResult()
        {
            var result = _controller.GetFullPrice();

            Assert.IsType<ObjectResult>(result);
        }

        #endregion

        #region post

        [Fact]
        public void Post_ActionExecutes_ReturnsOkObjectResult()
        {
            var result = _controller.Post(1);

            Assert.IsType<OkObjectResult>(result);
        }

        #endregion

        #region delete

        [Fact]
        public void Delete_ExistingIdPassed_ReturnsOkObjectResult()
        {
            var result = _controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        #endregion

        #region put

        [Fact]
        public void UpdateCartDishes_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = _controller.UpdateCartDishes(new List<CartDishesUpdateModel>());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Put_NullObjectPassed_ReturnsReturnsBadRequestObjectResult()
        {
            var result = _controller.UpdateCartDishes(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion
    }
}

