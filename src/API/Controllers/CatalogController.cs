using API.Interfaces;
using API.Models.Catalog;
using AutoMapper;
using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _сatalogService;
        private readonly ICatalogHelper _catalogHelper;

        public CatalogController(ICatalogService сatalogService,
             ICatalogHelper catalogHelper)
        {
            _сatalogService = сatalogService;
            _catalogHelper = catalogHelper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<CatalogDTO> сatalogDTOs = _сatalogService.GetСatalogs();

            return new ObjectResult(сatalogDTOs);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {           
           var catalog = _сatalogService.GetСatalog(id);

           if (catalog == null)
                return NotFound("Catalog not found");

            return new ObjectResult(catalog);
        }

        [HttpGet, Route("provider/{providerid}")]
        public IActionResult GetByProviderId(int providerid)
        {
            try
            {
                var catalog = _сatalogService.GetСatalogs(providerid);

                return new ObjectResult(catalog);
            }
            catch (ValidationException ex)
            {
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
            }
            catch (ValidationException ex)
            {
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

                return Ok(model);
            }
            catch (ValidationException ex)
            {
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

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
