using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models.Menu;
using Web.Interfaces;
using Core.Constants;
using System.Security.Claims;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IProviderService _providerService;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "menu";
        public MenuController(IMenuService menuService,
            IProviderService providerService,
            ILoggerService loggerService)
        {
            _menuService = menuService;
            _providerService = providerService;
            _loggerService = loggerService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(int providerId, string searchSelectionString, string seacrhString, SortState sortMenu = SortState.DateAsc)
        {
            IEnumerable<MenuDTO> menusDtos = _menuService.GetMenus(providerId);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MenuDTO, MenuViewModel>()).CreateMapper();
            var menus = mapper.Map<IEnumerable<MenuDTO>, List<MenuViewModel>>(menusDtos);

            var provider = _providerService.GetProvider(providerId);

            if (provider == null)
                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = "Provider not found" });

            ViewData["NameProvider"] = string.Empty + provider.Name;

            // list search
            List<string> searchSelection = new List<string>() { "SearchBy", "Info", "Date add" };

            seacrhString = seacrhString ?? string.Empty;

            // search
            if (searchSelectionString != string.Empty && searchSelectionString != null && searchSelectionString != "Search" && seacrhString != null)
            {
                if (searchSelectionString.ToLower() == searchSelection[1].ToLower() && seacrhString != string.Empty)
                {
                    menus = menus.Where(p => p.Info != null && p.Info.ToLower().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[1].ToLower() && seacrhString == string.Empty)
                {
                    menus = menus.Where(p => p.Info == null || p.Info == string.Empty).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[2].ToLower() && seacrhString != string.Empty)
                {
                    menus = menus.Where(p => p.Date != null && p.Date.ToShortDateString().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[2].ToLower() && seacrhString == string.Empty)
                {
                    menus = menus.Where(p => p.Date == null).ToList();
                }
            }

            ViewBag.DateSort = sortMenu == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;

            menus = sortMenu switch
            {
                SortState.DateDesc => menus.OrderByDescending(s => s.Date).ToList(),
                _ => menus.OrderBy(s => s.Date).ToList(),
            };

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX + $"/{providerId}", LoggerConstants.TYPE_GET, $"index – get menus of provider id: {providerId}", GetCurrentUserId());

            return View(new MenuAndProviderIdViewModel()
            {
                Menus = menus,
                ProviderId = providerId,
                SeacrhString = seacrhString,
                SearchSelection = new SelectList(searchSelection),
                SearchSelectionString = searchSelectionString
            });
        }

        #region For admin

        [HttpGet]
        public IActionResult Add(int providerId, string searchSelectionString, string seacrhString, SortState sortMenu)
        {
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.DateSort = sortMenu == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_ADD + $"/{providerId}", LoggerConstants.TYPE_GET, $"add menu provider id: {providerId}", GetCurrentUserId());

            return View(new AddMenuViewModel() { ProviderId = providerId, Date = DateTime.Now});
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel model, string searchSelectionString, string seacrhString, SortState sortMenu)
        {
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.DateSort = sortMenu;

            if (ModelState.IsValid)
            {
                MenuDTO menuDTO = new MenuDTO()
                {
                    Info = model.Info,
                    ProviderId = model.ProviderId,
                    Date = model.Date
                };

                try
                {
                    _menuService.AddMenu(menuDTO);

                    _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_ADD, LoggerConstants.TYPE_POST, $"add menu date: {model.Date} provider id: {model.ProviderId} successful", GetCurrentUserId());

                    return RedirectToAction("Index", new { model.ProviderId, searchSelectionString, seacrhString, sortMenu });
                }
                catch (ValidationException ex)
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_ADD, LoggerConstants.TYPE_POST, $"add menu date: {model.Date} provider id: {model.ProviderId} error: {ex.Message}", GetCurrentUserId());

                    ModelState.AddModelError(ex.Property, ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id, int providerId, string searchSelectionString, string seacrhString, SortState sortMenu)
        {
            try
            {
                _menuService.DeleteMenu(id);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE, LoggerConstants.TYPE_POST, $"delete menu id: {id} provider id: {providerId} error: {ex.Message}", GetCurrentUserId());

                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
            }

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE, LoggerConstants.TYPE_POST, $"delete menu id: {id} provider id: {providerId} successful", GetCurrentUserId());

            sortMenu = sortMenu == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;

            return RedirectToAction("Index", new { providerId, searchSelectionString, seacrhString, sortMenu });
        }

        [HttpGet]
        public IActionResult Edit(int id, string searchSelectionString, string seacrhString, SortState sortMenu)
        {
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.DateSort = sortMenu == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;

            MenuDTO menuDTO = _menuService.GetMenu(id);

            if (menuDTO == null)
                return RedirectToAction("Error", "Home", new { requestId = "400" , errorInfo = "Menu not found" });

            var provider = new EditMenuViewModel()
            {
                Id = menuDTO.Id,
                Date = menuDTO.Date,
                Info = menuDTO.Info,
                ProviderId = menuDTO.ProviderId
            };

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT + $"/{id}", LoggerConstants.TYPE_GET, $"edit menu id: {id} provider id: {provider.ProviderId}", GetCurrentUserId());

            return View(provider);
        }

        [HttpPost]
        public IActionResult Edit(EditMenuViewModel model, string searchSelectionString, string seacrhString, SortState sortMenu)
        {
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.DateSort = sortMenu;

            if (ModelState.IsValid)
            {
                MenuDTO menuDto = new MenuDTO
                {
                    Id = model.Id,
                    Date = model.Date,
                    Info = model.Info,
                    ProviderId = model.ProviderId
                };

                try
                {
                    _menuService.EditMenu(menuDto);

                    _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, $"edit menu id: {model.Id} provider id: {model.ProviderId} successful", GetCurrentUserId());

                    return RedirectToAction("Index", new { model.ProviderId, searchSelectionString, seacrhString, sortMenu });
                }
                catch (ValidationException ex)
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, $"edit menu id: {model.Id} provider id: {model.ProviderId} error: {ex.Message}", GetCurrentUserId());

                    ModelState.AddModelError(ex.Property, ex.Message);
                }
            }

            return View(model);
        }
       
        #endregion

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
