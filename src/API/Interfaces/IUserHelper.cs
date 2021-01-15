using API.Models.Identity.Account;
using Core.Identity;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUserHelper
    {
        Task<string> GetUserIdByEmailAsync(string email);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<bool> CheckLoginAsync(LoginModel model);
    }
}
