using API.Interfaces;
using API.Models.Identity.Account;
using Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers.Identity
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtConfigurator _jwtConfigurator;
        private readonly IUserHelper _userHelper;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IJwtConfigurator jwtConfigurator,
             IUserHelper userHelper
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtConfigurator = jwtConfigurator;
            _userHelper = userHelper;
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel user)
        {
            if (user == null)
                return BadRequest("Invalid client request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userHelper.CheckLoginAsync(user).Result)
                return Unauthorized();

            string userId = await _userHelper.GetUserIdByEmailAsync(user.Email);

            var tokenString = _jwtConfigurator.GetToken(userId);

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
                return Ok(model);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet, Route("profile")]
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

            return new ObjectResult(model);
        }

        [HttpPost, Route("editprofile"), Authorize]
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
                return Ok(model);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost, Route("changepassword"), Authorize]
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
