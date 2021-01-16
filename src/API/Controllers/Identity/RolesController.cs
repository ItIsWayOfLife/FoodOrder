using API.Models.Identity.Roles;
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

namespace API.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "api/roles";

        public RolesController(RoleManager<IdentityRole> roleManager,
              UserManager<ApplicationUser> userManager,
               ILoggerService loggerService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _loggerService = loggerService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var allRoles = _roleManager.Roles.ToList();
            List<string> allRolesStr = new List<string>();

            foreach (var role in allRoles)
            {
                allRolesStr.Add(role.Name);
            }
            _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_GET, $"get roles", GetCurrentUserId());

            return new ObjectResult(allRolesStr);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (id == null)
                return BadRequest("Invalid client request");

            // get user
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            // get lost role users
            var userRoles = await _userManager.GetRolesAsync(user);

            _loggerService.LogInformation(CONTROLLER_NAME +$"/{id}", LoggerConstants.TYPE_GET, $"get role id: {id}", GetCurrentUserId());

            return new ObjectResult(userRoles);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserChangeRoles model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            // get user
            ApplicationUser user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound("User not found");

            // get list roles user
            var userRoles = await _userManager.GetRolesAsync(user);

            // get list roles user, which were added
            var addedRoles = model.Roles.Except(userRoles);

            // get list roles that have been removed
            var removedRoles = userRoles.Except(model.Roles);

            await _userManager.AddToRolesAsync(user, addedRoles);

            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"edit roles user id: {user.Id}", GetCurrentUserId());

            return Ok(model);
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
