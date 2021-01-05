using Core.Constants;
using Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.Models.Users;

namespace Web.Controllers.Identity
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _loggerService;
        private readonly IUserHelper _userHelper;

        private const string CONTROLLER_NAME = "users";

        public UsersController(UserManager<ApplicationUser> userManager,
              ILoggerService loggerService,
              IUserHelper userHelper)
        {
            _userManager = userManager;
            _loggerService = loggerService;
            _userHelper = userHelper;
        }

        [HttpGet]
        public IActionResult Index(string searchSelectionString, string seacrhString = "")
        {
            var listUsers = _userManager.Users;
            var listViewUsers = new List<UserViewModel>();

            foreach (var listUser in listUsers)
            {
                listViewUsers.Add(
                    new UserViewModel()
                    {
                        Id = listUser.Id,
                        Email = listUser.Email,
                        FLP = $"{listUser.Lastname} {listUser.Firstname} {listUser.Patronomic}"
                    });
            }

            List<string> searchSelection = new List<string>() { "Search by", "Id", "Email", "Full name" };

            if (seacrhString == null)
            {
                seacrhString = "";
            }

            if (searchSelectionString == searchSelection[1])
            {
                listViewUsers = listViewUsers.Where(e => e.Id.ToLower().Contains(seacrhString.ToLower())).ToList();
            }
            else if (searchSelectionString == searchSelection[2])
            {
                listViewUsers = listViewUsers.Where(e => e.Email.ToLower().Contains(seacrhString.ToLower())).ToList();
            }
            else if (searchSelectionString == searchSelection[3])
            {
                listViewUsers = listViewUsers.Where(e => e.FLP.ToLower().Contains(seacrhString.ToLower())).ToList();
            }

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX, LoggerConstants.TYPE_GET, "index", GetCurrentUserId());

            return View(new UserFilterListViewModel()
            {
                ListUsers = new ListUserViewModel { Users = listViewUsers },
                SearchSelection = new SelectList(searchSelection),
                SeacrhString = seacrhString,
                SearchSelectionString = searchSelectionString
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_CREATE, LoggerConstants.TYPE_GET, "create", GetCurrentUserId());

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Patronomic = model.Patronomic
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_CREATE, LoggerConstants.TYPE_POST, $"create user {_userHelper.GetIdUserByEmail(model.Email)} successful", GetCurrentUserId());

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_CREATE, LoggerConstants.TYPE_POST, $"code:{error.Code}|description:{error.Description}", GetCurrentUserId());

                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT + $"/{id}", LoggerConstants.TYPE_GET, LoggerConstants.ERROR_USER_NOT_FOUND, GetCurrentUserId());

                return RedirectToAction("Error", "Home", new { requestId = "400" });
            }

            EditUserViewModel model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Patronomic = user.Patronomic
            };

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_GET, $"edit user {id}", GetCurrentUserId());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Firstname = model.Firstname;
                    user.Lastname = model.Lastname;
                    user.Patronomic = model.Patronomic;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, $"edit user {model.Id} successful", GetCurrentUserId());

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, $"code:{error.Code}|description:{error.Description}", GetCurrentUserId());

                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, LoggerConstants.ERROR_USER_NOT_FOUND, GetCurrentUserId());

                    return RedirectToAction("Error", "Home", new { requestId = "400" });
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE + $"/{id}", LoggerConstants.TYPE_POST, $"delete user {id} successful", GetCurrentUserId());

                    return RedirectToAction("Index");
                }
            }

            _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_DELETE, LoggerConstants.TYPE_POST, LoggerConstants.ERROR_USER_NOT_FOUND, GetCurrentUserId());

            return RedirectToAction("Error", "Home", new { requestId = "400" });
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction("Error", "Home", new { requestId = "400" });
            }

            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_CHANGEPASSWORD + $"/{id}", LoggerConstants.TYPE_GET, $"change password user {user.Id}", GetCurrentUserId());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_CHANGEPASSWORD, LoggerConstants.TYPE_POST, $"change password user {user.Id}", GetCurrentUserId());

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_CHANGEPASSWORD, LoggerConstants.TYPE_POST, $"code:{error.Code}|description:{error.Description}", GetCurrentUserId());

                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_CHANGEPASSWORD, LoggerConstants.TYPE_POST, LoggerConstants.ERROR_USER_NOT_FOUND, GetCurrentUserId());

                    ModelState.AddModelError(string.Empty, "User is not found");
                }
            }

            return View(model);
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
