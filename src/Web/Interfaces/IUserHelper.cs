using Core.Identity;

namespace Web.Interfaces
{
    public interface IUserHelper
    {
        string GetIdUserById(string id);
        string GetIdUserByEmail(string email);

        bool CheckUserExists(string id);
        ApplicationUser GetUserById(string id);
    }
}
