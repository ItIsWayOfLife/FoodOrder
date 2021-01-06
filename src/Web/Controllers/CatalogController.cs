using Core.DTO;
using Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Web.Models.Catalog;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _сatalogService;
        private readonly IProviderService _providerService;

        public CatalogController(ICatalogService сatalogService,
            IProviderService providerService)
        {
            _сatalogService = сatalogService;
            _providerService = providerService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(int providerId, int? menuId, string searchSelectionString, string name, SortState sortCatalog = SortState.NameAsc)
        {
            IEnumerable<CatalogDTO> сatalogDTOs = _сatalogService.GetСatalogs(providerId);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<CatalogDTO, CatalogViewModel>()).CreateMapper();
            var catalogs = mapper.Map<IEnumerable<CatalogDTO>, List<CatalogViewModel>>(сatalogDTOs);

            var provider = _providerService.GetProvider(providerId);

            if (provider == null)
                return RedirectToAction("Error", "Home", new { requestId = "400" });

            ViewData["NameProvider"] = "" + provider.Name;

            // list search
            List<string> searchSelection = new List<string>() { "SearchBy", "Catalog", "Info" };

            if (name == null)
                name = "";

            // search
            if (searchSelection[1] == searchSelectionString)
                catalogs = catalogs.Where(n => n.Name.ToLower().Contains(name.ToLower())).ToList();
            else if (searchSelection[2] == searchSelectionString)
                catalogs = catalogs.Where(e => e.Info.ToLower().Contains(name.ToLower())).ToList();

            ViewBag.NameSort = sortCatalog == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;

            catalogs = sortCatalog switch
            {
                SortState.NameDesc => catalogs.OrderByDescending(s => s.Name).ToList(),
                _ => catalogs.OrderBy(s => s.Name).ToList(),
            };

            return View(new CatalogdProviderIdViewModel()
            {
                MenuId = menuId,
                Catalogs = catalogs,
                ProviderId = providerId,
                SeacrhString = name,
                SearchSelection = new SelectList(searchSelection),
                SearchSelectionString = searchSelectionString
            });
        }

        #region For admin

        [HttpGet]
        public IActionResult Add(int providerId)
        {
            return View(new AddCatalogViewModel() { ProviderId = providerId });
        }

        [HttpPost]
        public IActionResult Add(AddCatalogViewModel model)
        {
            if (ModelState.IsValid)
            {
                CatalogDTO сatalogDTO = new CatalogDTO()
                {
                    Info = model.Info,
                    Name = model.Name,
                    ProviderId = model.ProviderId
                };

                _сatalogService.AddСatalog(сatalogDTO);

                return RedirectToAction("Index", new { model.ProviderId });
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int? id, int providerId, string searchSelectionString, string name)
        {
            _сatalogService.DeleteСatalog(id);

            return RedirectToAction("Index", new { providerId, searchSelectionString, name });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            CatalogDTO сatalogDTO = _сatalogService.GetСatalog(id);

            var provider = new EditCatalogViewModel()
            {
                Id = сatalogDTO.Id,
                Info = сatalogDTO.Info,
                Name = сatalogDTO.Name,
                ProviderId = сatalogDTO.ProviderId
            };

            return View(provider);
        }

        [HttpPost]
        public IActionResult Edit(EditCatalogViewModel model)
        {
            if (ModelState.IsValid)
            {
                CatalogDTO сatalogDTO = new CatalogDTO
                {
                    Id = model.Id,
                    Name = model.Name,
                    Info = model.Info,
                    ProviderId = model.ProviderId
                };

                _сatalogService.EditСatalog(сatalogDTO);

                return RedirectToAction("Index", new { providerId = model.ProviderId });
            }

            return View(model);
        }

        #endregion
    }
}
