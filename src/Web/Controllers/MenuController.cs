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

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IProviderService _providerService;

        public MenuController(IMenuService menuService,
            IProviderService providerService)
        {
            _menuService = menuService;
            _providerService = providerService;
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
                return RedirectToAction("Error", "Home", new { requestId = "400" });

            ViewData["NameProvider"] = "" + provider.Name;

            // list search
            List<string> searchSelection = new List<string>() { "SearchBy", "Info", "Date add" };

            if (seacrhString == null)
                seacrhString = "";

            // search
            if (searchSelectionString == searchSelection[1])
                menus = menus.Where(e => e.Info.ToLower().Contains(seacrhString.ToLower())).ToList();
            else if (searchSelectionString == searchSelection[2])
                menus = menus.Where(t => t.Date.ToShortDateString().Contains(seacrhString.ToLower())).ToList();

            ViewData["DateSort"] = sortMenu == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;

            menus = sortMenu switch
            {
                SortState.DateDesc => menus.OrderByDescending(s => s.Date).ToList(),
                _ => menus.OrderBy(s => s.Date).ToList(),
            };

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
        public IActionResult Add(int providerId)
        {
            return View(new AddMenuViewModel() { ProviderId = providerId, Date = DateTime.Now});
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel model)
        {
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
                }
                catch(ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);

                    return View(model);
                }

                return RedirectToAction("Index", new { model.ProviderId });
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int? id, int providerId, string searchSelectionString, string seacrhString)
        {
            try
            {
                _menuService.DeleteMenu(id);

                return RedirectToAction("Index", new { providerId, searchSelectionString, seacrhString });
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { requestId = "400" });
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                MenuDTO menuDTO = _menuService.GetMenu(id);

                if (menuDTO == null)
                    return RedirectToAction("Error", "Home", new { requestId = "400" });

                var provider = new EditMenuViewModel()
                {
                    Id = menuDTO.Id,
                    Date = menuDTO.Date,
                    Info = menuDTO.Info,
                    ProviderId = menuDTO.ProviderId
                };

                return View(provider);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { requestId = "400" });
            }
        }

        [HttpPost]
        public IActionResult Edit(EditMenuViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MenuDTO menuDto = new MenuDTO
                    {
                        Id = model.Id,
                        Date = model.Date,
                        Info = model.Info,
                        ProviderId = model.ProviderId
                    };

                    _menuService.EditMenu(menuDto);

                    return RedirectToAction("Index", new { providerId = model.ProviderId });
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);

                    return View(model);
                }
            }

            return View(model);
        }

        #endregion
    }
}
