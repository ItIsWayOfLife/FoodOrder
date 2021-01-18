using API.Controllers;
using API.Interfaces;
using API.Models.Menu;
using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace API.UnitTests.Controllers
{
    public class MenuControllerUnitTests
    {
        private readonly Mock<IMenuService> _mockMenuService;
        private readonly Mock<IMenuHelper> _mockMenuHelper;
        private readonly Mock<ILoggerService> _mockLoggerService;

        private readonly MenuController _controller;

        public MenuControllerUnitTests()
        {
            _mockMenuService = new Mock<IMenuService>();
            _mockMenuHelper = new Mock<IMenuHelper>();
            _mockLoggerService = new Mock<ILoggerService>();

            _controller = new MenuController(
                _mockMenuService.Object,
                _mockMenuHelper.Object,
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
            _mockMenuService.Setup(service => service.GetMenu(1))
                .Returns((MenuDTO)null);

            var result = _controller.Get(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsObjectResult()
        {
            _mockMenuService.Setup(service => service.GetMenu(1))
                .Returns(new MenuDTO());

            var result = _controller.Get(1);

            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void GetByProviderId_ExistingIdPassed_ReturnsObjectResult()
        {
            _mockMenuService.Setup(service => service.GetMenu(1))
                .Returns(new MenuDTO());

            var result = _controller.GetByProviderId(1);

            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void GetDishesInMenu_ExistingIdPassed_ReturnsObjectResult()
        {
            _mockMenuService.Setup(service => service.GetMenuIdDishes(1))
                .Returns(new List<int>());

            var result = _controller.GetDishesInMenu(1);

            Assert.IsType<ObjectResult>(result);
        }

        #endregion

        #region post

        [Fact]
        public void Post_ActionExecutes_ReturnsOkObjectResult()
        {
            var result = _controller.Post(new MenuModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Post_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Date", "Name is required");

            var result = _controller.Post(new MenuModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Post_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var result = _controller.Post(new MenuModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Post_NullObjectPassed_ReturnsReturnsBadRequestObjectResult()
        {
            var result = _controller.Post(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void MakeMenu_ActionExecutes_ReturnsOkObjectResult()
        {
            var result = _controller.MakeMenu(new MakeMenuModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void MakeMenu_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("MenuId", "MenuId is required");

            var result = _controller.MakeMenu(new MakeMenuModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void MakeMenu_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var result = _controller.MakeMenu(new MakeMenuModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void MakeMenu_NullObjectPassed_ReturnsReturnsBadRequestObjectResult()
        {
            var result = _controller.MakeMenu(null);

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

        [Fact]
        public void Delete_InvalidIdPassed_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Id", "Provider not found");

            var result = _controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        #endregion

        #region put

        [Fact]
        public void Put_ActionExecutes_ReturnsOkObjectResult()
        {
            var result = _controller.Put(new MenuModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Put_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Date", "Date is required");

            var result = _controller.Put(new MenuModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Put_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var result = _controller.Put(new MenuModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Put_NullObjectPassed_ReturnsReturnsBadRequestObjectResult()
        {
            var result = _controller.Post(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion
    }
}
