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

            ViewData["NameCatalog"] = "" + catalog.Name;

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

            if (seacrhString == null)
                seacrhString = "";

            // search
            if (searchSelectionString == searchSelection[1])
                dishes = dishes.Where(n => n.Name.ToLower().Contains(seacrhString.ToLower())).ToList();
            else if (searchSelectionString == searchSelection[2])
                dishes = dishes.Where(e => e.Info.ToLower().Contains(seacrhString.ToLower())).ToList();
            else if (searchSelectionString == searchSelection[3])
                dishes = dishes.Where(t => t.Weight.ToString() == seacrhString).ToList();
            else if (searchSelectionString == searchSelection[4])
                dishes = dishes.Where(t => t.Price.ToString() == seacrhString).ToList();

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
        public IActionResult Add(int catalogId)
        {
            return View(new AddDishViewModel() { CatalogId = catalogId });
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile uploadedFile, [FromForm] AddDishViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DishDTO dishDTO = null;
                    string path = null;

                    // save img
                    if (uploadedFile != null)
                    {
                        path = uploadedFile.FileName;
                        // сохраняем файл в папку wwwroot/files/dishes/
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

                    _dishService.AddDish(dishDTO);

                    return RedirectToAction("Index", new { dishDTO.CatalogId });
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                }
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(int? id, int catalogId, string searchSelectionString, string name)
        {
            try
            {
                _dishService.DeleteDish(id);

                return RedirectToAction("Index", new { catalogId, searchSelectionString, name });
            }
            catch (ValidationException ex)
            {
                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            try
            {
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
            catch (ValidationException ex)
            {
                return Content(ex.Message);
            }
        }


        [HttpPost]
        public async Task<ActionResult> Edit(IFormFile uploadedFile, [FromForm] EditDithViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DishDTO dishDTO = null;
                    string path = null;

                    // сохранение картинки
                    if (uploadedFile != null)
                    {
                        path = uploadedFile.FileName;
                        // сохраняем файл в папку files/provider/ в каталоге wwwroot
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
                        Path = path.Replace(_path, ""),
                        Price = model.Price,
                        Weight = model.Weight,
                        CatalogId = model.CatalogId
                    };

                    _dishService.EditDish(dishDTO);

                    return RedirectToAction("Index", new { dishDTO.CatalogId });
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
