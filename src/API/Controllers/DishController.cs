using API.Interfaces;
using API.Models.Dish;
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
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        private readonly IMenuService _menuService;
        private readonly IDishHelper _dishHelper;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "api/dish";

        private readonly string _path;

        public DishController(IDishService dishService,
            IMenuService menuService,
            IDishHelper dishHelper,
            ILoggerService loggerService)
        {
            _dishService = dishService;
            _menuService = menuService;
            _dishHelper = dishHelper;
            _loggerService = loggerService;

            _path = PathAPIConstants.API_URL + PathAPIConstants.API_PATH_FILES;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<DishDTO> dishDTOs = _dishService.GetAllDishes();
            var dishModels = _dishHelper.ConvertDishDTOsToDishModels(dishDTOs);

            _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_GET, "get dishes", GetCurrentUserId());

            return new ObjectResult(dishModels);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var dish = _dishService.GetDish(id);

            if (dish == null)
                return NotFound("Dish not found");

            _loggerService.LogInformation(CONTROLLER_NAME +$"/{id}", LoggerConstants.TYPE_GET, $"get dish id: {id}", GetCurrentUserId());

            return new ObjectResult(_dishHelper.ConvertDishDTOToDishModel(dish));
        }

        [HttpGet, Route("catalog/{catalogid}")]
        public IActionResult GetByCatalogId(int catalogid)
        {
            IEnumerable<DishDTO> dishDTOs = _dishService.GetDishes(catalogid);

            try
            {
                var dishModels = _dishHelper.ConvertDishDTOsToDishModels(dishDTOs);

                _loggerService.LogInformation(CONTROLLER_NAME + $"/catalog/{catalogid}", LoggerConstants.TYPE_GET, $"get dishes catalog id: {catalogid} successful", GetCurrentUserId());

                return new ObjectResult(dishModels);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/catalog/{catalogid}", LoggerConstants.TYPE_GET, $"get dishes catalog id: {catalogid} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("menudishes/{menuId}")]
        public IActionResult GetDishesByMenuId(int menuId)
        {
            try
            {
                var menuDishesDTOs = _menuService.GetMenuDishes(menuId).ToList();
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MenuDishesDTO, MenuDishesModel>()).CreateMapper();
                var menuDishes = mapper.Map<IEnumerable<MenuDishesDTO>, List<MenuDishesModel>>(menuDishesDTOs);

                for (int i = 0; i < menuDishesDTOs.Count(); i++)
                {
                    menuDishes[i].Path = _path + menuDishesDTOs[i].Path;
                    menuDishes[i].DishId = menuDishesDTOs[i].DishId.Value;
                }

                _loggerService.LogInformation(CONTROLLER_NAME + $"/menudishes/{menuId}", LoggerConstants.TYPE_GET, $"get  menu dishes menu id: {menuId} successful", GetCurrentUserId());

                return new ObjectResult(menuDishes);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/menudishes/{menuId}", LoggerConstants.TYPE_GET, $"get  menu dishes menu id: {menuId} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post(DishModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _dishService.AddDish(_dishHelper.ConvertDishModelToDishDTO(model));

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"add dish name: {model.Name} successful", GetCurrentUserId());
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"add dish name: {model.Name} error: {ex.Message}", GetCurrentUserId());

                return BadRequest(ex.Message);
            }

            return Ok(model);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult Put(DishModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _dishService.EditDish(_dishHelper.ConvertDishModelToDishDTO(model));

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"edit dish id: {model.Id} successful", GetCurrentUserId());
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"edit dish id: {model.Id} error: {ex.Message}", GetCurrentUserId());

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
                _dishService.DeleteDish(id);

                _loggerService.LogInformation(CONTROLLER_NAME +$"/{id}", LoggerConstants.TYPE_DELETE, $"delete dish id: {id} successful", GetCurrentUserId());
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/{id}", LoggerConstants.TYPE_DELETE, $"delete dish id: {id} error: {ex.Message}", GetCurrentUserId());

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
