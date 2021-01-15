using API.Models.Identity.Account;
using Core.Identity;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUserHelper
    {
        string GetUserIdByEmail(string email);
        ApplicationUser GetUserById(string id);
        Task<bool> CheckLogin(LoginModel model);
    }
}
