using API.Controllers;
using API.Interfaces;
using API.Models.Catalog;
using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace API.UnitTests.Controllers
{
    public class CatalogControllerUnitTests
    {
        private readonly Mock<ICatalogService> _mockCatalogService;
        private readonly Mock<ICatalogHelper> _mockCatalogHelper;
        private readonly Mock<ILoggerService> _mockLoggerService;

        private readonly CatalogController _controller;

        public CatalogControllerUnitTests()
        {
            _mockCatalogService = new Mock<ICatalogService>();
            _mockCatalogHelper = new Mock<ICatalogHelper>();
            _mockLoggerService = new Mock<ILoggerService>();

            _controller = new CatalogController(
                _mockCatalogService.Object,
                _mockCatalogHelper.Object,
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
        public void Get_WhenCalled_ReturnsAllItems()
        {
            _mockCatalogService.Setup(service => service.GetСatalogs())
                .Returns(new List<CatalogDTO>() { new CatalogDTO(), new CatalogDTO() });

            var result = _controller.Get();

            var objectResult = Assert.IsType<ObjectResult>(result);
            var catalogs = Assert.IsType<List<CatalogDTO>>(objectResult.Value);
            Assert.Equal(2, catalogs.Count);
        }

        [Fact]
        public void GetById_UnknownIdPassed_ReturnsNotFoundObjectResult()
        {
            _mockCatalogService.Setup(service => service.GetСatalog(1))
                .Returns((CatalogDTO)null);

            var result = _controller.Get(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsObjectResult()
        {
            _mockCatalogService.Setup(service => service.GetСatalog(1))
                .Returns(new CatalogDTO());

            var result = _controller.Get(1);

            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsRightItem()
        {
            _mockCatalogService.Setup(service => service.GetСatalog(1))
                .Returns(new CatalogDTO() { Id = 1});

            var result = _controller.Get(1) as ObjectResult;

            Assert.IsType<CatalogDTO>(result.Value);
            Assert.Equal(1, (result.Value as CatalogDTO).Id);
        }

        [Fact]
        public void GetByProviderId_ExistingIdPassed_ReturnsObjectResult()
        {
            _mockCatalogService.Setup(service => service.GetСatalog(1))
                .Returns(new CatalogDTO());

            var result = _controller.GetByProviderId(1);

            Assert.IsType<ObjectResult>(result);
        }

        #endregion

        #region post

        [Fact]
        public void Post_ActionExecutes_ReturnsOkObjectResult()
        {
            var result = _controller.Post(new CatalogModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Post_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = _controller.Post(new CatalogModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Post_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var result = _controller.Post(new CatalogModel());

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
            var result = _controller.Put(new CatalogModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Put_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Id", "Catalog not found");

            var result = _controller.Put(new CatalogModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Put_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var result = _controller.Put(new CatalogModel());

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
