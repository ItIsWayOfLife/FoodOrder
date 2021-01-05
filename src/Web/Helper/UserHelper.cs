using Core.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using Web.Interfaces;


namespace Web.Helper
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private const string USER_ANONYMOUS = "anonymous";

        public UserHelper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
    }
}
