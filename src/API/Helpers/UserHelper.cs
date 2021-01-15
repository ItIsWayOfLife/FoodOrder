using API.Interfaces;
using API.Models.Identity;
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

        public string GetUserIdByEmail(string email)
        {
            try
            {
                ApplicationUser user = _userManager.Users.FirstOrDefault(p => p.Email == email);

                if (user == null)
                    return null;

                return user.Id;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public ApplicationUser GetUserById(string id)
        {
            ApplicationUser user = _userManager.Users.FirstOrDefault(p => p.Id == id);

            return user;
        }

        [AllowAnonymous]
        public async Task<bool> CheckLogin(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
            return result.Succeeded;
        }
    }
}
