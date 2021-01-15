using API.Interfaces;
using API.Models.Provider;
using AutoMapper;
using Core.Constants;
using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IUserHelper _userHelper;
        private readonly IProviderHelper _providerHelper;

        private readonly string _path;

        public ProviderController(IProviderService providerService,
            IWebHostEnvironment appEnvironment,
            IUserHelper userHelper,
            IProviderHelper providerHelper)
        {
            _providerService = providerService;
            _appEnvironment = appEnvironment;
            _userHelper = userHelper;
            _providerHelper = providerHelper;

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

                return new ObjectResult(providers);          
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var provider = _providerService.GetProvider(id);

            if (provider == null)
                return NotFound("Provider not found");

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
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(model);
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
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(model);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                _providerService.DeleteProvider(id);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(id);           
        }
    }
}
