using Core.Constants;
using Core.DTO;
using Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Web.Models.MenuDishes;
using Core.Exceptions;
using Web.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    public class MenuDishesController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IProviderService _providerService;
        private readonly ICatalogService _catalogService;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "catalog";
        private readonly string _path;

        public MenuDishesController(IMenuService menuService,
            IProviderService providerService,
            ICatalogService catalogService,
            ILoggerService loggerService)
        {
            _menuService = menuService;
            _providerService = providerService;
            _path = PathConstants.PATH_DISH;
            _catalogService = catalogService;
            _loggerService = loggerService;
        }

        [HttpGet]
        public IActionResult Index(int menuId, string searchSelectionString, string seacrhString, string filterCatalog, SortStateMenuDishes sortMenuDish = SortStateMenuDishes.PriceAsc)
        {
            var menu = _menuService.GetMenu(menuId);

            if (menu == null)
            {
                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = "Menu not found" });
            }

            var provider = _providerService.GetProvider(menu.ProviderId);

            if (provider == null)
            {
                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = "Provider not found" });
            }

            ViewBag.NameMenuDishes = $"Menu dishes {provider.Name} On " + menu.Date.ToShortDateString();

            IEnumerable<MenuDishesDTO> menuDishesDTOs = _menuService.GetMenuDishes(menuId);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MenuDishesDTO, MenuDishesViewModel>()).CreateMapper();
            var menuDishes = mapper.Map<IEnumerable<MenuDishesDTO>, List<MenuDishesViewModel>>(menuDishesDTOs);

            List<int> catalogFilterId = new List<int>() { -1 };
            List<string> catalogFilterName = new List<string>() { "Filter" };

            // list search
            List<string> searchSelection = new List<string>() { "SearchBy", "Name", "Info", "Weight", "Price" };

            foreach (var mD in menuDishes)
            {
                mD.Path = _path + mD.Path;

                if (!catalogFilterId.Contains(mD.CatalogId))
                {
                    catalogFilterId.Add(mD.CatalogId);
                    catalogFilterName.Add(_catalogService.GetСatalog(mD.CatalogId).Name);
                }
            }

            // filter catalog
            if (filterCatalog != null && filterCatalog != catalogFilterName[0])
            {
                int idCatalogFilterName = catalogFilterName.IndexOf(filterCatalog);
                menuDishes = menuDishes.Where(p => p.CatalogId == catalogFilterId[idCatalogFilterName]).ToList();
            }

            seacrhString = seacrhString ?? string.Empty;

            // search
            if (searchSelectionString != string.Empty && searchSelectionString != null && searchSelectionString != "Search" && seacrhString != null)
            {
                if (searchSelectionString.ToLower() == searchSelection[1].ToLower() && seacrhString != string.Empty)
                {
                    menuDishes = menuDishes.Where(p =>p != null && p.Name.ToLower().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[1].ToLower() && seacrhString == string.Empty)
                {
                    menuDishes = menuDishes.Where(p => p.Name == null || p.Name == string.Empty).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[2].ToLower() && seacrhString != string.Empty)
                {
                    menuDishes = menuDishes.Where(p => p != null && p.Info.ToLower().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[2].ToLower() && seacrhString == string.Empty)
                {
                    menuDishes = menuDishes.Where(p => p.Info == null || p.Info == string.Empty).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[3].ToLower() && seacrhString != string.Empty)
                {
                    menuDishes = menuDishes.Where(p => p.Weight.ToString().Contains(seacrhString)).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[3].ToLower() && seacrhString == string.Empty)
                {
                    menuDishes = menuDishes.Where(p => p.Weight == 0).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[4].ToLower() && seacrhString != string.Empty)
                {
                    menuDishes = menuDishes.Where(p => p.Price.ToString().Contains(seacrhString)).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[4].ToLower() && seacrhString == string.Empty)
                {
                    menuDishes = menuDishes.Where(p => p.Price == 0).ToList();
                }
            }

            ViewBag.PriceSort = sortMenuDish == SortStateMenuDishes.PriceAsc ? SortStateMenuDishes.PriceDesc : SortStateMenuDishes.PriceAsc;

            menuDishes = sortMenuDish switch
            {
                SortStateMenuDishes.PriceDesc => menuDishes.OrderByDescending(s => s.Price).ToList(),
                _ => menuDishes.OrderBy(s => s.Price).ToList(),
            };

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX +$"/{menuId}", LoggerConstants.TYPE_GET, $"index – get menu dishes of menu id: {menuId}", GetCurrentUserId());

            return View(new ListMenuDishViewModel()
            {
                MenuId = menuId,
                MenuDishes = menuDishes,
                SeacrhString = seacrhString,
                SearchSelection = new SelectList(searchSelection),
                FilterCategorySelection = new SelectList(catalogFilterName),
                SearchSelectionString = searchSelectionString,
                ProviderId = menu.ProviderId,
                FilterCatalog = filterCatalog
            });
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Delete(int id, int menuId, string searchSelectionString, string seacrhString, string filterCatalog, SortStateMenuDishes sortMenuDish)
        {
            try
            {
                _menuService.DeleteDishInMenu(id);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE +$"/{id}", LoggerConstants.TYPE_POST, $"delete menu dish id: {id} menu id: {menuId} error: {ex.Message}", GetCurrentUserId());

                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
            }

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE + $"/{id}", LoggerConstants.TYPE_POST, $"delete menu dish id: {id} menu id: {menuId} successful", GetCurrentUserId());

            sortMenuDish = sortMenuDish == SortStateMenuDishes.PriceAsc ? SortStateMenuDishes.PriceDesc : SortStateMenuDishes.PriceAsc;

            return RedirectToAction("Index", new { menuId, searchSelectionString, seacrhString, filterCatalog, sortMenuDish });
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
