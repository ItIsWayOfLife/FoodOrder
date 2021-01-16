using API.Interfaces;
using API.Models.Menu;
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
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IMenuHelper _menuHelper;

        public MenuController(IMenuService menuService,
             IMenuHelper menuHelper)
        {
            _menuService = menuService;
            _menuHelper = menuHelper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<MenuDTO> menuDTOs = _menuService.GetAllMenus();
            var menuModels = _menuHelper.ConvertMenuDTOsToMenuModels(menuDTOs);

            return new ObjectResult(menuModels);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var menu = _menuService.GetMenu(id);

            if (menu == null)
                return NotFound("Menu not found");

            return new ObjectResult(_menuHelper.ConvertMenuDTOToMenuModel(menu));
        }

        [HttpGet, Route("provider/{providerid}")]
        public IActionResult GetByProviderId(int providerid)
        {
            try
            {
                IEnumerable<MenuDTO> menuDTOs = _menuService.GetMenus(providerid).OrderByDescending(p => p.Date);

                return new ObjectResult(_menuHelper.ConvertMenuDTOsToMenuModels(menuDTOs));
            }
            catch (ValidationException ex)
            {
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

                return new ObjectResult(arrayIdDishes);
            }
            catch (ValidationException ex)
            {
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

                return Ok(model);
            }
            catch (ValidationException ex)
            {
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

                return Ok(model);
            }
            catch (ValidationException ex)
            {
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
                _menuService.DeleteMenu(id);

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
