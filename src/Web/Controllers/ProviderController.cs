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

            if (searchSelectionString == "Id")
                providersViewModel = providersViewModel.Where(n => n.Id.ToString().ToLower().Contains(seacrhString.ToLower())).ToList();
            else if (searchSelectionString == "Name")
                providersViewModel = providersViewModel.Where(n => n.Name.ToLower().Contains(seacrhString.ToLower())).ToList();
            else if (searchSelectionString == "Email")
                providersViewModel = providersViewModel.Where(e => e.Email.ToLower().Contains(seacrhString.ToLower())).ToList();
            else if (searchSelectionString == "TimeWorkWith")
                providersViewModel = providersViewModel.Where(t => t.TimeWorkWith.ToShortTimeString().ToLower().Contains(seacrhString.ToLower())).ToList();
            else if (searchSelectionString == "TimeWorkTo")
                providersViewModel = providersViewModel.Where(t => t.TimeWorkTo.ToShortTimeString().ToLower().Contains(seacrhString.ToLower())).ToList();
            else if (searchSelectionString == "IsActive")
                providersViewModel = providersViewModel.Where(a => a.IsActive == true).ToList();
            else if (searchSelectionString == "Inactive")
                providersViewModel = providersViewModel.Where(a => a.IsActive == false).ToList();

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX, LoggerConstants.TYPE_GET, "index", GetCurrentUserId());

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

            _loggerService.LogInformation(CONTROLLER_NAME + "/listfavoriteproviders", LoggerConstants.TYPE_GET, "index", GetCurrentUserId());

            return View(new ListProviderViewModel() { Providers = providersViewModel });
        }

        #region For admin

        [HttpGet]
        public ActionResult Add()
        {
            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_ADD, LoggerConstants.TYPE_GET, "add", GetCurrentUserId());

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile uploadedFile, [FromForm] AddProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                ProviderDTO providerDto = null;
                string path = "";

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

                _providerService.AddProvider(providerDto);

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_ADD, LoggerConstants.TYPE_POST, $"add {model.Email}", GetCurrentUserId());

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _providerService.DeleteProvider(id);

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE, LoggerConstants.TYPE_POST +$"/{id}", $"delete {id} provider", GetCurrentUserId());

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ProviderDTO providerDto = _providerService.GetProvider(id);

            if (providerDto == null)
                return RedirectToAction("Error", "Home", new { requestId = "400" });

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

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT +$"/{id}", LoggerConstants.TYPE_GET, "edit", GetCurrentUserId());

            return View(provider);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(IFormFile uploadedFile, [FromForm] EditProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                ProviderDTO providerDto = null;
                string path = "";

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
                    Path = path.Replace(_path, ""),
                    TimeWorkTo = model.TimeWorkTo,
                    TimeWorkWith = model.TimeWorkWith,
                    WorkingDays = model.WorkingDays
                };

                _providerService.EditProvider(providerDto);

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, $"edit {model.Id} provider", GetCurrentUserId());

                return RedirectToAction("Index");
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
