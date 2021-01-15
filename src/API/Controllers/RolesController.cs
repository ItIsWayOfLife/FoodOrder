using API.Models.Identity;
using Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager,
              UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
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

            return new ObjectResult(allRolesStr);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (id == null)
                return BadRequest();

            // get user
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            // get lost role users
            var userRoles = await _userManager.GetRolesAsync(user);

            return new ObjectResult(userRoles);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserChangeRoles model)
        {
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

            return Ok(model);
        }
    }
}
