using API.Interfaces;
using API.Models.Menu;
using Core.Constants;
using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBaseGetUserId
    {
        private readonly IMenuService _menuService;
        private readonly IMenuHelper _menuHelper;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "api/menu";

        public MenuController(IMenuService menuService,
             IMenuHelper menuHelper,
             ILoggerService loggerService)
        {
            _menuService = menuService;
            _menuHelper = menuHelper;
            _loggerService = loggerService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<MenuDTO> menuDTOs = _menuService.GetAllMenus();
            var menuModels = _menuHelper.ConvertMenuDTOsToMenuModels(menuDTOs);

            _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_GET, "get menus", GetCurrentUserId());

            return new ObjectResult(menuModels);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var menu = _menuService.GetMenu(id);

            if (menu == null)
                return NotFound("Menu not found");

            _loggerService.LogInformation(CONTROLLER_NAME +$"/{id}", LoggerConstants.TYPE_GET, $"get menu id: {id}", GetCurrentUserId()); ;

            return new ObjectResult(_menuHelper.ConvertMenuDTOToMenuModel(menu));
        }

        [HttpGet, Route("provider/{providerid}")]
        public IActionResult GetByProviderId(int providerid)
        {
            try
            {
                IEnumerable<MenuDTO> menuDTOs = _menuService.GetMenus(providerid).OrderByDescending(p => p.Date);

                _loggerService.LogInformation(CONTROLLER_NAME + $"/provider/{providerid}", LoggerConstants.TYPE_GET, $"get menu providerid: {providerid} successful", GetCurrentUserId());

                return new ObjectResult(_menuHelper.ConvertMenuDTOsToMenuModels(menuDTOs));
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/provider/{providerid}", LoggerConstants.TYPE_GET, $"get menu providerid: {providerid} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("dishes/{menuid}")]
        public IActionResult GetDishesInMenu(int menuid)
        {
            try
            {
                List<int> arrayIdDishes = new List<int>();

                arrayIdDishes = _menuService.GetMenuIdDishes(menuid);

                _loggerService.LogInformation(CONTROLLER_NAME + $"/dishes/{menuid}", LoggerConstants.TYPE_GET, $"get dishes in menu menuid: {menuid} successful", GetCurrentUserId());

                return new ObjectResult(arrayIdDishes);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/dishes/{menuid}", LoggerConstants.TYPE_GET, $"get dishes in menu menuid: {menuid} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post(MenuModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _menuService.AddMenu(_menuHelper.ConvertMenuModelToMenuDTO(model));

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"add menu date: {model.Date} successful", GetCurrentUserId());

                return Ok(model);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"add menu date: {model.Date} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("makemenu")]
        [Authorize(Roles = "admin")]
        public IActionResult MakeMenu(MakeMenuModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _menuService.MakeMenu(model.MenuId, model.NewAddedDishes, model.AllSelect);

                _loggerService.LogInformation(CONTROLLER_NAME + "/makemenu", LoggerConstants.TYPE_POST, $"make menu id: {model.MenuId} successful", GetCurrentUserId());

                return Ok(model);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + "/makemenu", LoggerConstants.TYPE_POST, $"make menu id: {model.MenuId} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult Put(MenuModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _menuService.EditMenu(_menuHelper.ConvertMenuModelToMenuDTO(model));

                _loggerService.LogInformation(CONTROLLER_NAME , LoggerConstants.TYPE_PUT, $"edit menu id: {model.Id} successful", GetCurrentUserId());

                return Ok(model);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"edit menu id: {model.Id} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                _menuService.DeleteMenu(id);

                _loggerService.LogInformation(CONTROLLER_NAME +$"{id}", LoggerConstants.TYPE_DELETE, $"delete menu id: {id} successful", GetCurrentUserId());

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"{id}", LoggerConstants.TYPE_DELETE, $"delete menu id: {id} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }
    }
}
