using Core.DTO;
using Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Web.Models.Catalog;
using Web.Interfaces;
using Core.Constants;
using System.Security.Claims;
using Core.Exceptions;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _сatalogService;
        private readonly IProviderService _providerService;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "catalog";

        public CatalogController(ICatalogService сatalogService,
            IProviderService providerService,
            ILoggerService loggerService)
        {
            _сatalogService = сatalogService;
            _providerService = providerService;
            _loggerService = loggerService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(int providerId, int? menuId, string searchSelectionString, string seacrhString, SortState sortCatalog = SortState.NameAsc)
        {
            IEnumerable<CatalogDTO> сatalogDTOs = _сatalogService.GetСatalogs(providerId);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<CatalogDTO, CatalogViewModel>()).CreateMapper();
            var catalogs = mapper.Map<IEnumerable<CatalogDTO>, List<CatalogViewModel>>(сatalogDTOs);

            var provider = _providerService.GetProvider(providerId);

            if (provider == null)
                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = "Provider not found" });

            ViewData["NameProvider"] = "" + provider.Name;

            // list search
            List<string> searchSelection = new List<string>() { "SearchBy", "Catalog", "Info" };

            if (seacrhString == null)
                seacrhString = "";

            // search
            if (searchSelection[1] == searchSelectionString)
                catalogs = catalogs.Where(n => n.Name.ToLower().Contains(seacrhString.ToLower())).ToList();
            else if (searchSelection[2] == searchSelectionString)
                catalogs = catalogs.Where(e => e.Info.ToLower().Contains(seacrhString.ToLower())).ToList();

            ViewBag.NameSort = sortCatalog == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;

            catalogs = sortCatalog switch
            {
                SortState.NameDesc => catalogs.OrderByDescending(s => s.Name).ToList(),
                _ => catalogs.OrderBy(s => s.Name).ToList(),
            };

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX + $"/{providerId}", LoggerConstants.TYPE_GET, $"index – get catalogs of provider id: {providerId}", GetCurrentUserId());

            return View(new CatalogdProviderIdViewModel()
            {
                MenuId = menuId,
                Catalogs = catalogs,
                ProviderId = providerId,
                SeacrhString = seacrhString,
                SearchSelection = new SelectList(searchSelection),
                SearchSelectionString = searchSelectionString
            });
        }

        #region For admin

        [HttpGet]
        public IActionResult Add(int providerId, int? menuId, string searchSelectionString, string seacrhString, SortState sortCatalog)
        {
            ViewBag.MenuId = menuId;
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.NameSort = sortCatalog == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_ADD + $"/{providerId}", LoggerConstants.TYPE_GET, $"add catalog provider id: {providerId}", GetCurrentUserId());

            return View(new AddCatalogViewModel() { ProviderId = providerId });
        }

        [HttpPost]
        public IActionResult Add(AddCatalogViewModel model, int? menuId, string searchSelectionString, string seacrhString, SortState sortCatalog)
        {
            ViewBag.MenuId = menuId;
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.NameSort = sortCatalog;

            if (ModelState.IsValid)
            {
                CatalogDTO сatalogDTO = new CatalogDTO()
                {
                    Info = model.Info,
                    Name = model.Name,
                    ProviderId = model.ProviderId
                };

                try
                {
                    _сatalogService.AddСatalog(сatalogDTO);
                }
                catch (ValidationException ex)
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_ADD, LoggerConstants.TYPE_POST, $"add catalog name: {model.Name} provider id: {model.ProviderId} error: {ex.Message}", GetCurrentUserId());

                    ModelState.AddModelError(ex.Property, ex.Message);

                    return View(model);
                }

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_ADD , LoggerConstants.TYPE_POST, $"add catalog name: {model.Name} provider id: {model.ProviderId} successful", GetCurrentUserId());
              
                ViewBag.NameSort = sortCatalog == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;

                return RedirectToAction("Index", new { model.ProviderId, menuId, searchSelectionString, seacrhString, sortCatalog });
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id, int providerId, string searchSelectionString, string seacrhString, int? menuId, SortState sortCatalog)
        {
            try
            {
                _сatalogService.DeleteСatalog(id);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE, LoggerConstants.TYPE_POST, $"delete catalog id: {id} provider id: {providerId} error: {ex.Message}", GetCurrentUserId());

                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
            }

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE, LoggerConstants.TYPE_POST, $"delete catalog id: {id} provider id: {providerId} successful", GetCurrentUserId());

            sortCatalog = sortCatalog == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;

            return RedirectToAction("Index", new { providerId, menuId, searchSelectionString, seacrhString, sortCatalog });
        }

        [HttpGet]
        public ActionResult Edit(int id, int? menuId, string searchSelectionString, string seacrhString, SortState sortCatalog)
        {
            ViewBag.MenuId = menuId;
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.NameSort = sortCatalog == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;

            CatalogDTO сatalogDTO = _сatalogService.GetСatalog(id);

            var provider = new EditCatalogViewModel()
            {
                Id = сatalogDTO.Id,
                Info = сatalogDTO.Info,
                Name = сatalogDTO.Name,
                ProviderId = сatalogDTO.ProviderId
            };

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT +$"/{id}", LoggerConstants.TYPE_GET, $"edit catalog id: {id} provider id: {provider.ProviderId}", GetCurrentUserId());

            return View(provider);
        }

        [HttpPost]
        public IActionResult Edit(EditCatalogViewModel model, int? menuId, string searchSelectionString, string seacrhString, SortState sortCatalog)
        {
            ViewBag.MenuId = menuId;
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;
            ViewBag.NameSort = sortCatalog;

            if (ModelState.IsValid)
            {
                CatalogDTO сatalogDTO = new CatalogDTO
                {
                    Id = model.Id,
                    Name = model.Name,
                    Info = model.Info,
                    ProviderId = model.ProviderId
                };

                try
                {
                    _сatalogService.EditСatalog(сatalogDTO);
                }
                catch (ValidationException ex)
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, $"edit catalog id: {model.Id} provider id: {model.ProviderId} error: {ex.Message}", GetCurrentUserId());

                    ModelState.AddModelError(ex.Property, ex.Message);

                    return View(model);
                }

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, $"edit catalog id: {model.Id} provider id: {model.ProviderId} successful", GetCurrentUserId());

                ViewBag.NameSort = sortCatalog == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;

                return RedirectToAction("Index", new { model.ProviderId, menuId, searchSelectionString, seacrhString, sortCatalog });
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
