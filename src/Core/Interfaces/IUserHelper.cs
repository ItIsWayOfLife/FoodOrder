using Core.Identity;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserHelper
    {
        string GetIdUserById(string id);
        string GetIdUserByEmail(string email);

        bool CheckUserExists(string id);
        ApplicationUser GetUserById(string id);

        Task<string> GetUserIdByEmailAsync(string email);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<bool> CheckLoginAsync(string email, string password);
    }
}
