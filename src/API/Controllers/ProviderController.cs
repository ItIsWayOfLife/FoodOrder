using API.Interfaces;
using API.Models.Provider;
using AutoMapper;
using Core.Constants;
using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;
        private readonly IProviderHelper _providerHelper;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "api/provider";

        private readonly string _path;

        public ProviderController(IProviderService providerService,
            IProviderHelper providerHelper,
            ILoggerService loggerService)
        {
            _providerService = providerService;
            _providerHelper = providerHelper;
            _loggerService = loggerService;

            _path = _path = PathAPIConstants.API_URL + PathAPIConstants.API_PATH_FILES;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<ProviderDTO> providersDtos = _providerService.GetProviders();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ProviderDTO, ProviderModel>()).CreateMapper();
            var providers = mapper.Map<IEnumerable<ProviderDTO>, List<ProviderModel>>(providersDtos);

            foreach (var pr in providers)
            {
                pr.Path = _path + pr.Path;
                pr.TimeWorkTo = providersDtos.FirstOrDefault(p => p.Id == pr.Id).TimeWorkTo.ToString("HH:mm");
                pr.TimeWorkWith = providersDtos.FirstOrDefault(p => p.Id == pr.Id).TimeWorkWith.ToString("HH:mm");
            }

            _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_GET, $"get providers", GetCurrentUserId());

            return new ObjectResult(providers);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var provider = _providerService.GetProvider(id);

            if (provider == null)
                return NotFound("Provider not found");

            _loggerService.LogInformation(CONTROLLER_NAME +$"/{id}", LoggerConstants.TYPE_GET, $"get provider id: {id}", GetCurrentUserId());

            return new ObjectResult(provider);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post(ProviderModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _providerService.AddProvider(_providerHelper.ConvertProviderModelToProviderDTO(model));

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"add provider name: {model.Name} successful", GetCurrentUserId());

                return Ok(model);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"add provider name: {model.Name} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult Put(ProviderModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var provider = _providerHelper.ConvertProviderModelToProviderDTO(model);

            try
            {
                _providerService.EditProvider(provider);

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"edit provider id: {model.Id} successful", GetCurrentUserId());

                return Ok(model);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"edit provider id: {model.Id} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                _providerService.DeleteProvider(id);

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_DELETE, $"delete provider id: {id} successful", GetCurrentUserId());
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_DELETE, $"delete provider id: {id} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }

            return Ok(id);           
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
