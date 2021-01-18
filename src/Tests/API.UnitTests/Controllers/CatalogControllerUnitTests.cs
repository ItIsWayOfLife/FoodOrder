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
            // Act
            var result = _controller.Get();

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsAllItems()
        {
            // Arrange
            _mockCatalogService.Setup(service => service.GetСatalogs())
                .Returns(new List<CatalogDTO>() { new CatalogDTO(), new CatalogDTO() });

            // Act
            var result = _controller.Get();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var providers = Assert.IsType<List<CatalogDTO>>(objectResult.Value);
            Assert.Equal(2, providers.Count);
        }

        [Fact]
        public void GetById_UnknownIdPassed_ReturnsNotFoundObjectResult()
        {
            // Arrange
            int id = 1;

            _mockCatalogService.Setup(service => service.GetСatalog(id))
                .Returns((CatalogDTO)null);

            // Act
            var result = _controller.Get(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsObjectResult()
        {
            // Arrange
            int id = 1;

            _mockCatalogService.Setup(service => service.GetСatalog(id))
                .Returns(new CatalogDTO() { Id = 1 });

            // Act
            var result = _controller.Get(id);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsRightItem()
        {
            // Arrange
            int id = 1;

            _mockCatalogService.Setup(service => service.GetСatalog(id))
                .Returns(new CatalogDTO() { Id = 1 });

            // Act
            var result = _controller.Get(id) as ObjectResult;

            // Assert
            Assert.IsType<CatalogDTO>(result.Value);
            Assert.Equal(id, (result.Value as CatalogDTO).Id);
        }

        [Fact]
        public void GetByProviderId_ExistingIdPassed_ReturnsObjectResult()
        {
            // Arrange
            int id = 1;

            _mockCatalogService.Setup(service => service.GetСatalog(id))
                .Returns(new CatalogDTO() { Id = 1 });

            // Act
            var result = _controller.GetByProviderId(id);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        #endregion

        #region post

        [Fact]
        public void Post_ActionExecutes_ReturnsOkObjectResult()
        {
            _mockCatalogService.Setup(service => service.AddСatalog(new CatalogDTO()));

            var result = _controller.Post(new CatalogModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Post_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");
            var catalog = new CatalogModel { Id = 1, Info = "info" };

            var result = _controller.Post(catalog);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Post_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var provider = new CatalogModel { Id = 1, ProviderId = 2,  Name = "name", Info = "info" };

            var result = _controller.Post(provider);

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
            // Arrange
            int id = 1;

            _mockCatalogService.Setup(service => service.DeleteСatalog(id));

            // Act
            var result = _controller.Delete(id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Delete_InvalidIdPassed_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Id", "Catalog not found");

            // Arrange
            int id = 1;

            _mockCatalogService.Setup(service => service.DeleteСatalog(id));

            // Act
            var result = _controller.Delete(id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        #endregion

        #region put

        [Fact]
        public void Put_ActionExecutes_ReturnsOkObjectResult()
        {
            _mockCatalogService.Setup(service => service.AddСatalog(new CatalogDTO()));

            var result = _controller.Put(new CatalogModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Put_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");
            var catalog = new CatalogModel { Id = 1, ProviderId =2, Info = "info" };

            var result = _controller.Put(catalog);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Put_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var catalog = new CatalogModel { Id = 1, ProviderId =2, Name = "name", Info = "info" };

            var result = _controller.Put(catalog);

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
