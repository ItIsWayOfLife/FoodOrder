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


    }
}
