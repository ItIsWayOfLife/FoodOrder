using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Helper
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private const string USER_ANONYMOUS = "anonymous";

        public UserHelper(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string GetIdUserByEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return USER_ANONYMOUS;
            }

            ApplicationUser user = _userManager.Users.FirstOrDefault(p => p.Email == email);

            if (user == null)
            {
                return USER_ANONYMOUS;
            }

            return user.Id;
        }

        public string GetIdUserById(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return USER_ANONYMOUS;
            }

            ApplicationUser user = _userManager.Users.FirstOrDefault(p => p.Id == id);

            if (user == null)
            {
                return USER_ANONYMOUS;
            }

            return user.Id;
        }

        public bool CheckUserExists(string id)
        {
            ApplicationUser user = null;
            user = _userManager.Users.FirstOrDefault(p => p.Id == id);

            return user == null ? false : true;
        }

        public ApplicationUser GetUserById(string id)
        {
            ApplicationUser user = null;
            user = _userManager.Users.FirstOrDefault(p => p.Id == id);

            return user;
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
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            return user;
        }

        public async Task<bool> CheckLoginAsync(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, true, false);

            return result.Succeeded;
        }
    }
}
