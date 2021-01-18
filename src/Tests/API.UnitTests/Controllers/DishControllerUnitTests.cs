using API.Controllers;
using API.Interfaces;
using API.Models.Dish;
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
   public  class DishControllerUnitTests
    {
        private readonly Mock<IDishService> _mockDishService;
        private readonly Mock<IMenuService> _mockMenuService;
        private readonly Mock<IDishHelper> _mockDishHelper;
        private readonly Mock<ILoggerService> _mockLoggerService;

        private readonly DishController _controller;

        public DishControllerUnitTests()
        {
            _mockDishService = new Mock<IDishService>();
            _mockMenuService = new Mock<IMenuService>();
            _mockDishHelper = new Mock<IDishHelper>();
            _mockLoggerService = new Mock<ILoggerService>();

            _controller = new DishController(
                _mockDishService.Object,
                  _mockMenuService.Object,
                _mockDishHelper.Object,
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
        public void GetById_UnknownIdPassed_ReturnsNotFoundObjectResult()
        {
            _mockDishService.Setup(service => service.GetDish(1))
                .Returns((DishDTO)null);

            var result = _controller.Get(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsObjectResult()
        {
            _mockDishService.Setup(service => service.GetDish(1))
                .Returns(new DishDTO());

            var result = _controller.Get(1);

            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void GetByCatalogId_ExistingIdPassed_ReturnsObjectResult()
        {
            _mockDishService.Setup(service => service.GetDish(1))
                .Returns(new DishDTO());

            var result = _controller.GetByCatalogId(1);

            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void GetDishesByMenuId_ExistingIdPassed_ReturnsObjectResult()
        {
            _mockMenuService.Setup(service => service.GetMenuDishes(1))
                .Returns(new List<MenuDishesDTO>());

            var result = _controller.GetDishesByMenuId(1);

            Assert.IsType<ObjectResult>(result);
        }

        #endregion

        #region post

        [Fact]
        public void Post_ActionExecutes_ReturnsOkObjectResult()
        {
            var result = _controller.Post(new DishModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Post_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = _controller.Post(new DishModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Post_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var result = _controller.Post(new DishModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Post_NullObjectPassed_ReturnsReturnsBadRequestObjectResult()
        {
            var result = _controller.Post(null);

            Assert.IsType<BadRequestObjectResult>(result);
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
        public void Put_ActionExecutes_ReturnsOkObjectResult()
        {
            var result = _controller.Put(new DishModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Put_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = _controller.Put(new DishModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Put_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var result = _controller.Put(new DishModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Put_NullObjectPassed_ReturnsReturnsBadRequestObjectResult()
        {
            var result = _controller.Put(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion
    }
}
