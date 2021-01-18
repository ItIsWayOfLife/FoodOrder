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
    public class ProviderControllerUnitTests
    {
        private readonly Mock<IProviderService> _mockProviderService;
        private readonly Mock<IProviderHelper> _mockProviderHelper;
        private readonly Mock<ILoggerService> _mockLoggerService;

        private readonly ProviderController _controller;

        public ProviderControllerUnitTests()
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
            var result = _controller.Get();

            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsAllItems()
        {
            _mockProviderService.Setup(service => service.GetProviders())
                .Returns(new List<ProviderDTO>() { new ProviderDTO(), new ProviderDTO() });

            var result = _controller.Get();

            var objectResult = Assert.IsType<ObjectResult>(result);
            var providers = Assert.IsType<List<ProviderModel>>(objectResult.Value);
            Assert.Equal(2, providers.Count);
        }

        [Fact]
        public void GetById_UnknownIdPassed_ReturnsNotFoundObjectResult()
        {
            _mockProviderService.Setup(service => service.GetProvider(1))
                .Returns((ProviderDTO)null);

            var result = _controller.Get(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsObjectResult()
        {
            _mockProviderService.Setup(service => service.GetProvider(1))
                .Returns(new ProviderDTO());

            var result = _controller.Get(1);

            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsRightItem()
        {
            _mockProviderService.Setup(service => service.GetProvider(1))
                .Returns(new ProviderDTO() { Id = 1});

            var result = _controller.Get(1) as ObjectResult;

            Assert.IsType<ProviderDTO>(result.Value);
            Assert.Equal(1, (result.Value as ProviderDTO).Id);
        }

        #endregion

        #region post

        [Fact]
        public void Post_ActionExecutes_ReturnsOkObjectResult()
        {
            var result = _controller.Post(new ProviderModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Post_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = _controller.Post(new ProviderModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Post_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var result = _controller.Post(new ProviderModel());

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

        [Fact]
        public void Delete_InvalidIdPassed_ReturnsBadRequestObjectResult()
        {
            var result = _controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        #endregion

        #region put

        [Fact]
        public void Put_ActionExecutes_ReturnsOkObjectResult()
        {
            var result = _controller.Put(new ProviderModel());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Put_InvalidModelState_ReturnsBadRequestObjectResult()
        {
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = _controller.Put(new ProviderModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Put_ValidModelState_ReturnsBadRequestObjectResult()
        {
            var result = _controller.Put(new ProviderModel());

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
