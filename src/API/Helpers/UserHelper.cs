using API.Interfaces;
using API.Models.Identity.Account;
using Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    [Authorize]
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public UserHelper(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<string> GetUserIdByEmailAsync(string email)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                    return null;

                return user.Id;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            return user;
        }

        [AllowAnonymous]
        public async Task<bool> CheckLoginAsync(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

            return result.Succeeded;
        }
    }
}
