using API.Controllers;
using API.Interfaces;
using API.Models.Order;
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
    public class OrderControllerUnitTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<ICartService> _mockCartService;
        private readonly Mock<IOrderHelper> _mockOrderHelper;
        private readonly Mock<ILoggerService> _mockLoggerService;

        private readonly OrderController _controller;

        public OrderControllerUnitTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockCartService = new Mock<ICartService>();
            _mockOrderHelper = new Mock<IOrderHelper>();
            _mockLoggerService = new Mock<ILoggerService>();

            _controller = new OrderController(
                _mockOrderService.Object,
                _mockCartService.Object,
                _mockOrderHelper.Object,
                _mockLoggerService.Object);
        }

        #region get

        [Fact]
        public void Get_WhenCalled_ReturnsObjectResult()
        {
            var result = _controller.Get();

            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void GetDishe_WhenCalled_ReturnsObjectResult()
        {
            _mockOrderService.Setup(service => service.GetOrderDishes("", 1))
                .Returns(new List<OrderDishesDTO>() { new OrderDishesDTO(), new OrderDishesDTO() });

            var result = _controller.GetDishes(1);

            Assert.IsType<ObjectResult>(result);
        }

        #endregion

        #region post

        [Fact]
        public void Post_ActionExecutes_ReturnsOkObjectResult()
        {
            var result = _controller.Post();

            Assert.IsType<OkResult>(result);
        }

        #endregion
    }
}
