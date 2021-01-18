using API.Models.Identity.Users;
using AutoMapper;
using Core.Constants;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBaseGetUserId
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "api/users";

        public UsersController(UserManager<ApplicationUser> userManager,
              IUserHelper userHelper,
              ILoggerService loggerService)
        {
            _userManager = userManager;
            _userHelper = userHelper;
            _loggerService = loggerService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<ApplicationUser> listUsers = _userManager.Users;
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, UserModel>()).CreateMapper();
            var listViewUsers = mapper.Map<IEnumerable<ApplicationUser>, List<UserModel>>(listUsers);

            _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_GET, "get users", GetCurrentUserId());

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

            _loggerService.LogInformation(CONTROLLER_NAME + $"/{id}", LoggerConstants.TYPE_GET, "get user id: {id}", GetCurrentUserId());

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
                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"add user id: {await _userHelper.GetUserIdByEmailAsync(user.Email)} successful", GetCurrentUserId());

                return Ok(model);
            }
            else
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"add user error: {result.Errors}", GetCurrentUserId());

                return BadRequest(result.Errors);
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
                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"edit user id: {model.Id} successful", GetCurrentUserId());

                return Ok(model);
            }
            else
            {
                _loggerService.LogWarning(CONTROLLER_NAME, LoggerConstants.TYPE_PUT, $"edit user id: {model.Id} error: {result.Errors}", GetCurrentUserId());

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
                _loggerService.LogInformation(CONTROLLER_NAME + $"/{id}", LoggerConstants.ACTION_DELETE, $"delete user id: {id} successful", GetCurrentUserId());

                return Ok(user);
            }
            else
            {
                _loggerService.LogWarning(CONTROLLER_NAME + $"/{id}", LoggerConstants.ACTION_DELETE, $"delete user id: {id} error: {result.Errors}", GetCurrentUserId());

                return BadRequest(result.Errors);
            }
        }

        [HttpPut, Route("changepassword")]
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
                _loggerService.LogInformation(CONTROLLER_NAME + "/changepassword", LoggerConstants.TYPE_PUT, $"change password user id: {model.Id} successful", GetCurrentUserId());

                return Ok(model);
            }
            else
            {
                _loggerService.LogWarning(CONTROLLER_NAME + "/changepassword", LoggerConstants.TYPE_PUT, $"change password user id: {model.Id} error: {result.Errors}", GetCurrentUserId());

                return BadRequest(result.Errors);
            }
        }
    }
}
