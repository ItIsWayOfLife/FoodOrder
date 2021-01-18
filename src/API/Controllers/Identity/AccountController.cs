using API.Interfaces;
using API.Models.Identity.Account;
using Core.Constants;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBaseGetUserId
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtConfigurator _jwtConfigurator;
        private readonly IUserHelper _userHelper;
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "api/account";

        public AccountController(UserManager<ApplicationUser> userManager,
            IJwtConfigurator jwtConfigurator,
             IUserHelper userHelper,
             ILoggerService loggerService
            )
        {
            _userManager = userManager;
            _jwtConfigurator = jwtConfigurator;
            _userHelper = userHelper;
            _loggerService = loggerService;
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel user)
        {
            if (user == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userHelper.CheckLoginAsync(user.Email, user.Password).Result)
                return Unauthorized();

            string userId = await _userHelper.GetUserIdByEmailAsync(user.Email);

            var tokenString = _jwtConfigurator.GetToken(userId);

            if (tokenString != null)
            {
                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTIN_LOGIN, LoggerConstants.TYPE_POST, $"login user id: {userId} successful", GetCurrentUserId());
            }
            else
            {
                _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTIN_LOGIN, LoggerConstants.TYPE_POST, $"login user id: {userId} error", GetCurrentUserId());
            }

            return Ok(new { Token = tokenString });
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register(RegisterModel model)
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

            // add user
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTIN_REGISTER, LoggerConstants.TYPE_POST, $"register user id: {await _userHelper.GetUserIdByEmailAsync(user.Email)} successful", GetCurrentUserId());

                return Ok(model);
            }
            else
            {
                _loggerService.LogWarning(CONTROLLER_NAME + LoggerConstants.ACTIN_REGISTER, LoggerConstants.TYPE_POST, $"register user id: {await _userHelper.GetUserIdByEmailAsync(user.Email)} error: {result.Errors}", GetCurrentUserId());

                return BadRequest(result.Errors);
            }
        }

        [HttpGet, Route("profile"), Authorize]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            ApplicationUser user = await _userHelper.GetUserByIdAsync(GetCurrentUserId());

            if (user == null)
                return NotFound("User not found");

            ProfileModel model = new ProfileModel()
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Patronymic = user.Patronymic,
                Email = user.Email
            };

            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTIN_PROFILE, LoggerConstants.TYPE_GET, $"get profile", user.Id);

            return new ObjectResult(model);
        }

        [HttpPut, Route("editprofile")]
        public async Task<IActionResult> Edit(ProfileModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ApplicationUser user = await _userHelper.GetUserByIdAsync(GetCurrentUserId());

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
                _loggerService.LogInformation(CONTROLLER_NAME + "/editprofile", LoggerConstants.TYPE_PUT, $"edit profile user id: {user.Id} successful", user.Id);

                return Ok(model);
            }
            else
            {
                _loggerService.LogWarning(CONTROLLER_NAME + "/editprofile", LoggerConstants.TYPE_PUT, $"edit profile user id: {user.Id} error: {result.Errors}", user.Id);

                return BadRequest(result.Errors);
            }
        }

        [HttpPut, Route("changepassword"), Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            if (model == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ApplicationUser user = await _userHelper.GetUserByIdAsync(GetCurrentUserId());

            if (user == null)
                return NotFound("User not found");

            IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                _loggerService.LogInformation(CONTROLLER_NAME + "/changepassword", LoggerConstants.TYPE_PUT, $"edit password user id: {user.Id} successful", user.Id);

                return Ok(model);
            }
            else
            {
                _loggerService.LogWarning(CONTROLLER_NAME + "/changepassword", LoggerConstants.TYPE_PUT, $"edit profile user id: {user.Id} successful", user.Id);

                return BadRequest(result.Errors);
            }
        }
    }
}
