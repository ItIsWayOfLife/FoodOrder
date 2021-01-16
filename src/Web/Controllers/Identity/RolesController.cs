using Core.Constants;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Models.Roles;

namespace Web.Controllers.Identity
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "roles";

        public RolesController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILoggerService loggerService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _loggerService = loggerService;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId, string searchSelectionString, string seacrhString)
        {
            // get users
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                // get lost roles users
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT + $"/{userId}", LoggerConstants.TYPE_GET, $"edit roles user id: {user.Id}", GetCurrentUserId());

                ViewBag.SearchSelectionString = searchSelectionString;
                ViewBag.SeacrhString = seacrhString;

                return View(model);
            }

            return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = "User not found" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles, string searchSelectionString, string seacrhString)
        {
            // get users
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            ViewBag.SearchSelectionString = searchSelectionString;
            ViewBag.SeacrhString = seacrhString;

            if (user != null)
            {
                // get list roles users
                var userRoles = await _userManager.GetRolesAsync(user);

                // get list roles users, which were added
                var addedRoles = roles.Except(userRoles);

                // get roles, which have been removed
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, $"edit roles user id: {user.Id} successful", GetCurrentUserId());

                return RedirectToAction("Index", "Users", new { searchSelectionString, seacrhString });
            }

            _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTION_EDIT, LoggerConstants.TYPE_POST, LoggerConstants.ERROR_USER_NOT_FOUND, null);

            return RedirectToAction("Error", "Home", new { requestId = "400", errorInfo = "User not found" });
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
