using Core.Constants;
using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.Provider;
using Web.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Core.Exceptions;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProviderController : Controller
    {
        private readonly IProviderService _providerService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IProviderHelper _providerHelper;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "book";
        private readonly string _path;

        public ProviderController(IProviderService providerService,
            IWebHostEnvironment appEnvironment,
             IProviderHelper providerHelper,
             ILoggerService loggerService)
        {
            _providerService = providerService;
            _appEnvironment = appEnvironment;
            _path = PathConstants.PATH_PROVIDER;
            _providerHelper = providerHelper;
            _loggerService = loggerService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(string searchSelectionString, string seacrhString)
        {
            var providersViewModel = _providerHelper.GetProviders();

            List<string> searchSelection = new List<string>();

            if (User.Identity.IsAuthenticated && User.IsInRole("admin"))
            {
                searchSelection = _providerHelper.GetSearchSelection(true);
            }
            else
            {
                searchSelection = _providerHelper.GetSearchSelection(false);
            }

            seacrhString = seacrhString ?? string.Empty;

            // search
            if (searchSelectionString != string.Empty && searchSelectionString != null && searchSelectionString != "Search" && seacrhString != null)
            {
                if (searchSelectionString.ToLower() == "id" && seacrhString != string.Empty)
                {
                    providersViewModel = providersViewModel.Where(p => p.Id.ToString().ToLower().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == "id" && seacrhString == string.Empty)
                {
                    providersViewModel = providersViewModel.Where(p => p.Id == 0).ToList();
                }
                else if (searchSelectionString.ToLower() == "name" && seacrhString != string.Empty)
                {
                    providersViewModel = providersViewModel.Where(p => p.Name != null && p.Name.ToLower().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == "name" && seacrhString == string.Empty)
                {
                    providersViewModel = providersViewModel.Where(p => p.Name == null || p.Name == string.Empty).ToList();
                }
                else if (searchSelectionString.ToLower() == "email" && seacrhString != string.Empty)
                {
                    providersViewModel = providersViewModel.Where(p => p.Email != null && p.Email.ToLower().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == "email" && seacrhString == string.Empty)
                {
                    providersViewModel = providersViewModel.Where(p => p.Email == null || p.Email == string.Empty).ToList();
                }
                else if (searchSelectionString.ToLower() == "timeworkwith" && seacrhString != string.Empty)
                {
                    providersViewModel = providersViewModel.Where(p => p.TimeWorkWith != null && p.TimeWorkWith.ToShortTimeString().ToLower().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == "timeworkwith" && seacrhString == string.Empty)
                {
                    providersViewModel = providersViewModel.Where(p => p.TimeWorkWith == null).ToList();
                }
                else if (searchSelectionString.ToLower() == "timeworkto" && seacrhString != string.Empty)
                {
                    providersViewModel = providersViewModel.Where(p => p.TimeWorkTo != null && p.TimeWorkTo.ToShortTimeString().ToLower().Contains(seacrhString.ToLower())).ToList();
                }
                else if (searchSelectionString.ToLower() == "timeworkto" && seacrhString != string.Empty)
                {
                    providersViewModel = providersViewModel.Where(p => p.TimeWorkTo == null).ToList();
                }
                else if (searchSelectionString.ToLower() == "isactive")
                {
                    providersViewModel = providersViewModel.Where(a => a.IsActive == true).ToList();
                }
                else if (searchSelectionString.ToLower() == "inactive")
                {
                    providersViewModel = providersViewModel.Where(a => a.IsActive == false).ToList();
                }
            }

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX, LoggerConstants.TYPE_GET, "index – get providers", GetCurrentUserId());

            return View(new ProviderListViewModel()
            {
                ListProviders = new ListProviderViewModel() { Providers = providersViewModel },
                SeacrhString = seacrhString,
                SearchSelection = new SelectList(searchSelection),
                SearchSelectionString = searchSelectionString
            });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ListFavoriteProviders()
        {
            var providersViewModel = _providerHelper.GetProvidersFavorite();

            _loggerService.LogInformation(CONTROLLER_NAME + "/listfavoriteproviders", LoggerConstants.TYPE_GET, "get favorite providers", GetCurrentUserId());

            return View(new ListProviderViewModel() { Providers = providersViewModel });
        }

        #region For admin

        [HttpGet]
        public IActionResult Add(string searchSelectionString, string seacrhString)
        {
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_ADD, LoggerConstants.TYPE_GET, "add", GetCurrentUserId());

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile uploadedFile, [FromForm] AddProviderViewModel model, string searchSelectionString, string seacrhString)
        {
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;

            if (ModelState.IsValid)
            {
                ProviderDTO providerDto = null;
                string path = string.Empty;

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

                providerDto = new ProviderDTO
                {
                    Email = model.Email,
                    Info = model.Info,
                    IsActive = model.IsActive,
                    IsFavorite = model.IsFavorite,
                    Name = model.Name,
                    Path = path,
                    TimeWorkTo = model.TimeWorkTo,
                    TimeWorkWith = model.TimeWorkWith,
                    WorkingDays = model.WorkingDays
                };

                try
                {
                    _providerService.AddProvider(providerDto);

                    _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_ADD, LoggerConstants.TYPE_POST, $"add provider email: {model.Email} successful", GetCurrentUserId());

                    return RedirectToAction("Index", new { searchSelectionString, seacrhString });
                }
                catch (ValidationException ex)
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_ADD, LoggerConstants.TYPE_POST, $"add provider email: {model.Email} error: {ex.Message}", GetCurrentUserId());

                    ModelState.AddModelError(ex.Property, ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id, string searchSelectionString, string seacrhString)
        {
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;

            try
            {
                _providerService.DeleteProvider(id);
            }
            catch (ValidationException ex)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE, LoggerConstants.TYPE_POST + $"/{id}", $"delete provider id: {id} error: {ex.Message}", GetCurrentUserId());

                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = ex.Message });
            }

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE, LoggerConstants.TYPE_POST +$"/{id}", $"delete provider id: {id} successful", GetCurrentUserId());

            return RedirectToAction("Index", new { searchSelectionString, seacrhString });
        }

        [HttpGet]
        public IActionResult Edit(int id, string searchSelectionString, string seacrhString)
        {
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;

            ProviderDTO providerDto = _providerService.GetProvider(id);

            if (providerDto == null)
                return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = "Provider not found" });

            var provider = new EditProviderViewModel()
            {
                Id = providerDto.Id,
                Email = providerDto.Email,
                Info = providerDto.Info,
                IsActive = providerDto.IsActive,
                IsFavorite = providerDto.IsFavorite,
                Name = providerDto.Name,
                Path = _path + providerDto.Path,
                TimeWorkTo = providerDto.TimeWorkTo,
                TimeWorkWith = providerDto.TimeWorkWith,
                WorkingDays = providerDto.WorkingDays
            };

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT +$"/{id}", LoggerConstants.TYPE_GET, $"get provider id: {id} for edit", GetCurrentUserId());

            return View(provider);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IFormFile uploadedFile, [FromForm] EditProviderViewModel model, string searchSelectionString, string seacrhString)
        {
            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;

            if (ModelState.IsValid)
            {
                ProviderDTO providerDto = null;
                string path = string.Empty;

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

                providerDto = new ProviderDTO
                {
                    Id = model.Id,
                    Email = model.Email,
                    Info = model.Info,
                    IsActive = model.IsActive,
                    IsFavorite = model.IsFavorite,
                    Name = model.Name,
                    Path = path.Replace(_path, string.Empty),
                    TimeWorkTo = model.TimeWorkTo,
                    TimeWorkWith = model.TimeWorkWith,
                    WorkingDays = model.WorkingDays
                };

                try
                {
                    _providerService.EditProvider(providerDto);

                    _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, $"edit provider id: {model.Id} successful", GetCurrentUserId());

                    return RedirectToAction("Index", new { searchSelectionString, seacrhString });
                }
                catch (ValidationException ex)
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, $"edit provider id: {model.Id} error: {ex.Message}", GetCurrentUserId());

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
