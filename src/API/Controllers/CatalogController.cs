using API.Interfaces;
using API.Models.Catalog;
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
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _сatalogService;
        private readonly ICatalogHelper _catalogHelper;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "api/catalog";

        public CatalogController(ICatalogService сatalogService,
             ICatalogHelper catalogHelper,
             ILoggerService loggerService)
        {
            _сatalogService = сatalogService;
            _catalogHelper = catalogHelper;
            _loggerService = loggerService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<CatalogDTO> сatalogDTOs = _сatalogService.GetСatalogs();

            _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_GET, "get catalogs", GetCurrentUserId());

            return new ObjectResult(сatalogDTOs);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {           
           var catalog = _сatalogService.GetСatalog(id);

           if (catalog == null)
                return NotFound("Catalog not found");

            _loggerService.LogInformation(CONTROLLER_NAME +$"/{id}", LoggerConstants.TYPE_GET, $"get catalog id: {id}", GetCurrentUserId());

            return new ObjectResult(catalog);
        }

        [HttpGet, Route("provider/{providerid}")]
        public IActionResult GetByProviderId(int providerid)
        {
            try
            {
                var catalog = _сatalogService.GetСatalogs(providerid);

                _loggerService.LogInformation(CONTROLLER_NAME + $"/provider/{providerid}", LoggerConstants.TYPE_GET, $"get catalogs provider id: {providerid} successful", GetCurrentUserId());

                return new ObjectResult(catalog);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/provider/{providerid}", LoggerConstants.TYPE_GET, $"get catalogs provider id: {providerid} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post(CatalogModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _сatalogService.AddСatalog(_catalogHelper.ConvertCatalogModelToCatalogDTO(model));

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"add catalog name: {model.Name} successful", GetCurrentUserId());

            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"add catalog name: {model.Name} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }

            return Ok(model);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult Put(CatalogModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _сatalogService.EditСatalog(_catalogHelper.ConvertCatalogModelToCatalogDTO(model));

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"edit catalog id: {model.Id} successful", GetCurrentUserId());

                return Ok(model);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"edit catalog id: {model.Id} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                _сatalogService.DeleteСatalog(id);

                _loggerService.LogInformation(CONTROLLER_NAME +$"/{id}", LoggerConstants.TYPE_DELETE, $"delete catalog id: {id} successful", GetCurrentUserId());

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/{id}", LoggerConstants.TYPE_DELETE, $"delete catalog id: {id} error: {ex.Message}", GetCurrentUserId());

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
