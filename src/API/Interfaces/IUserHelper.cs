using Core.Identity;

namespace API.Interfaces
{
    public interface IUserHelper
    {
        string GetUserIdByEmail(string email);
        ApplicationUser GetUserById(string id);
    }
}
