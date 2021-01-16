using API.Interfaces;
using API.Models.Identity.Users;
using AutoMapper;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers.Identity
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;

        public UsersController(UserManager<ApplicationUser> userManager,
              IUserHelper userHelper)
        {
            _userManager = userManager;
            _userHelper = userHelper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<ApplicationUser> listUsers = _userManager.Users;
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, UserModel>()).CreateMapper();
            var listViewUsers = mapper.Map<IEnumerable<ApplicationUser>, List<UserModel>>(listUsers);

            return new ObjectResult(listViewUsers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            ApplicationUser user = await _userHelper.GetUserByIdAsync(id);

            if (user == null)
                return NotFound("User not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, UserModel>()).CreateMapper();
            UserModel userViewModel = mapper.Map<ApplicationUser, UserModel>(user);

            return new ObjectResult(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ApplicationUser user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Patronymic = model.Patronymic
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(model);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(UserModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ApplicationUser user = await _userHelper.GetUserByIdAsync(model.Id);

            if (user == null)
                return NotFound("User not found");

            user.Email = model.Email;
            user.UserName = model.Email;
            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Patronymic = model.Patronymic;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(model);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await _userHelper.GetUserByIdAsync(id);

            if (user == null)
                return NotFound("User not found");

            if (GetCurrentUserId() == user.Id)
                return BadRequest("User cannot delete himself");

            IdentityResult result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok(user);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost, Route("changepassword")]
        public async Task<IActionResult> ChangePassword(UserModelChangePasword model)
        {
            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ApplicationUser user = await _userHelper.GetUserByIdAsync(model.Id);

            if (user == null)
                return NotFound("User not found");

            IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(model);
            }
            else
            {
                return BadRequest(result.Errors);
            }
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
