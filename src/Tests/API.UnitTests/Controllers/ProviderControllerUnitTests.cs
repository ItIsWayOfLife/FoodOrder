using API.Controllers;
using API.Interfaces;
using API.Models.Provider;
using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace API.UnitTests.Controllers
{
    public class UsersControllerUnitTests
    {
        private readonly Mock<IProviderService> _mockProviderService;
        private readonly Mock<IProviderHelper> _mockProviderHelper;
        private readonly Mock<ILoggerService> _mockLoggerService;

        private readonly ProviderController _controller;

        public UsersControllerUnitTests()
        {
            _mockProviderService = new Mock<IProviderService>();
            _mockProviderHelper = new Mock<IProviderHelper>();
            _mockLoggerService = new Mock<ILoggerService>();

            _controller = new ProviderController(
                _mockProviderService.Object,
                _mockProviderHelper.Object,
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
            _mockProviderService.Setup(service => service.GetProviders())
                .Returns(new List<ProviderDTO>() { new ProviderDTO(), new ProviderDTO() });

            // Act
            var result = _controller.Get();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var providers = Assert.IsType<List<ProviderModel>>(objectResult.Value);
            Assert.Equal(2, providers.Count);
        }

        [Fact]
        public void GetById_UnknownIdPassed_ReturnsNotFoundObjectResult()
        {
            // Arrange
            int id = 1;

            _mockProviderService.Setup(service => service.GetProvider(id))
                .Returns((ProviderDTO)null);

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

            _mockProviderService.Setup(service => service.GetProvider(id))
                .Returns(new ProviderDTO() { Id = 1 });

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

            _mockProviderService.Setup(service => service.GetProvider(id))
                .Returns(new ProviderDTO() { Id = 1 });

            // Act
            var result = _controller.Get(id) as ObjectResult;

            // Assert
            Assert.IsType<ProviderDTO>(result.Value);
            Assert.Equal(id, (result.Value as ProviderDTO).Id);
        }

        #endregion

        #region post

        [Fact]
        public void Post_ActionExecutes_ReturnsOkObjectResult()
        {
            _mockProviderService.Setup(service => service.AddProvider(new ProviderDTO()));

            var result = _controller.Post(new ProviderModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Post_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");
            var provider = new ProviderModel { Id = 1, Email = "email", Info = "info" };

            var result = _controller.Post(provider);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Post_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var provider = new ProviderModel { Id = 1, Email = "email", Info = "info" };

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

            _mockProviderService.Setup(service => service.DeleteProvider(id));

            // Act
            var result = _controller.Delete(id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Delete_InvalidIdPassed_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Id", "Provider not found");

            // Arrange
            int id = 1;

            _mockProviderService.Setup(service => service.DeleteProvider(id));

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
            _mockProviderService.Setup(service => service.AddProvider(new ProviderDTO()));

            var result = _controller.Put(new ProviderModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Put_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");
            var provider = new ProviderModel { Id = 1, Email = "email", Info = "info" };

            var result = _controller.Put(provider);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Put_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var provider = new ProviderModel { Id = 1, Email = "email", Info = "info" };

            var result = _controller.Put(provider);

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
