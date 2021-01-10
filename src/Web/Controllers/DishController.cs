using Core.Constants;
using Core.DTO;
using Core.Exceptions;
using Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Models.Dish;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class DishController : Controller
    {
        private readonly IDishService _dishService;
        private readonly ICatalogService _сatalogService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IMenuService _menuService;

        private readonly string _path;

        public DishController(IDishService dishService, IWebHostEnvironment appEnvironment,
             ICatalogService сatalogService,
             IMenuService menuService)
        {
            _dishService = dishService;
            _appEnvironment = appEnvironment;
            _сatalogService = сatalogService;
            _menuService = menuService;
            _path = PathConstants.PATH_DISH;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(int catalogId, int? menuId, string searchSelectionString, string seacrhString, SortState sortDish = SortState.PriceAsc)
        {
            var catalog = _сatalogService.GetСatalog(catalogId);

            ViewBag.ProviderId = catalog.ProviderId;

            if (catalog == null)
                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = "Catalog not found" });

            ViewData["NameCatalog"] = string.Empty + catalog.Name;

            IEnumerable<DishDTO> providersDtos;
            List<int> addedDish = new List<int>();

            if (menuId != null)
            {
                addedDish = _menuService.GetMenuIdDishes(menuId);
                providersDtos = _dishService.GetDishesForMenu(catalogId, addedDish);
            }
            else
            {
                providersDtos = _dishService.GetDishes(catalogId);
            }

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DishDTO, DishViewModel>()).CreateMapper();
            var dishes = mapper.Map<IEnumerable<DishDTO>, List<DishViewModel>>(providersDtos);

            foreach (var d in dishes)
            {
                d.Path = _path + d.Path;
            }

            // list search
            List<string> searchSelection = new List<string>() { "SearchBy", "Name", "Info", "Weight", "Price" };

            seacrhString = seacrhString ?? string.Empty;

            // search
            if (searchSelectionString != string.Empty && searchSelectionString != null && searchSelectionString != "Search" && seacrhString != null)
            {
                if (searchSelectionString.ToLower() == searchSelection[1].ToLower() && seacrhString != string.Empty)
                {
                    dishes = dishes.Where(p => p.Name != null && p.Name.ToLower().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[1].ToLower() && seacrhString == string.Empty)
                {
                    dishes = dishes.Where(p => p.Name == null || p.Name == string.Empty).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[2].ToLower() && seacrhString != string.Empty)
                {
                    dishes = dishes.Where(p => p.Info != null && p.Info.ToLower().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[2].ToLower() && seacrhString == string.Empty)
                {
                    dishes = dishes.Where(p => p.Info == null || p.Info == string.Empty).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[3].ToLower() && seacrhString != string.Empty)
                {
                    dishes = dishes.Where(p => p.Weight.ToString().Contains(seacrhString)).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[3].ToLower() && seacrhString == string.Empty)
                {
                    dishes = dishes.Where(p => p.Weight == 0).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[4].ToLower() && seacrhString != string.Empty)
                {
                    dishes = dishes.Where(p => p.Price.ToString().Contains(seacrhString)).ToList();
                }
                else if (searchSelectionString.ToLower() == searchSelection[4].ToLower() && seacrhString == string.Empty)
                {
                    dishes = dishes.Where(p => p.Price == 0).ToList();
                }
            }

            ViewData["PriceSort"] = sortDish == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;

            dishes = sortDish switch
            {
                SortState.PriceDesc => dishes.OrderByDescending(s => s.Price).ToList(),
                _ => dishes.OrderBy(s => s.Price).ToList(),
            };

            return View(new ListDishViewModel()
            {
                MenuId = menuId,
                Dishes = dishes,
                CatalogId = catalogId,
                AddedDish = addedDish,
                SeacrhString = seacrhString,
                SearchSelection = new SelectList(searchSelection),
                SearchSelectionString = searchSelectionString
            });
        }

        #region For admin

        [HttpGet]
        public IActionResult Add(int catalogId, int? menuId, string searchSelectionString, string seacrhString, SortState sortDish)
        {
            ViewBag.MenuId = menuId;
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.SortDish = sortDish == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;

            return View(new AddDishViewModel() { CatalogId = catalogId });
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile uploadedFile, [FromForm] AddDishViewModel model, int? menuId, string searchSelectionString, string seacrhString, SortState sortDish)
        {
            ViewBag.MenuId = menuId;
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.SortDish = sortDish;

            if (ModelState.IsValid)
            {
                DishDTO dishDTO = null;
                string path = null;

                // save img
                if (uploadedFile != null)
                {
                    path = uploadedFile.FileName;
                    // save img to wwwroot/files/dishes/
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + _path + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                }

                dishDTO = new DishDTO
                {
                    Info = model.Info,
                    CatalogId = model.CatalogId,
                    Name = model.Name,
                    Path = path,
                    Price = model.Price,
                    Weight = model.Weight
                };

                try
                {
                    _dishService.AddDish(dishDTO);

                    return RedirectToAction("Index", new { model.CatalogId, menuId, searchSelectionString, seacrhString, sortDish });
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                }
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(int id, int catalogId, int? menuId, string searchSelectionString, string seacrhString, SortState sortDish)
        {
            try
            {
                _dishService.DeleteDish(id);

                sortDish = sortDish == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;

                return RedirectToAction("Index", new { catalogId, menuId, searchSelectionString, seacrhString , sortDish });
            }
            catch (ValidationException ex)
            {
                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Edit(int id, int? menuId, string searchSelectionString, string seacrhString, SortState sortDish)
        {
            ViewBag.MenuId = menuId;
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.SortDish = sortDish == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;

            DishDTO dishDTO = _dishService.GetDish(id);

            var provider = new EditDithViewModel()
            {
                Info = dishDTO.Info,
                Id = dishDTO.Id,
                Name = dishDTO.Name,
                Path = _path + dishDTO.Path,
                Price = dishDTO.Price,
                Weight = dishDTO.Weight,
                CatalogId = dishDTO.CatalogId
            };

            return View(provider);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(IFormFile uploadedFile, [FromForm] EditDithViewModel model, int? menuId, string searchSelectionString, string seacrhString, SortState sortDish)
        {
            ViewBag.MenuId = menuId;
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.SortDish = sortDish;

            if (ModelState.IsValid)
            {
                DishDTO dishDTO = null;
                string path = null;

                // save img
                if (uploadedFile != null)
                {
                    path = uploadedFile.FileName;
                    // save img to wwwroot/files/provider/
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + _path + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    path = model.Path;
                }

                dishDTO = new DishDTO
                {
                    Id = model.Id,
                    Info = model.Info,
                    Name = model.Name,
                    Path = path.Replace(_path, string.Empty),
                    Price = model.Price,
                    Weight = model.Weight,
                    CatalogId = model.CatalogId
                };

                try
                {
                    _dishService.EditDish(dishDTO);

                    return RedirectToAction("Index", new { dishDTO.CatalogId, menuId, searchSelectionString, seacrhString, sortDish });
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                }
            }

            return View(model);
        }

        #endregion
    }
}
