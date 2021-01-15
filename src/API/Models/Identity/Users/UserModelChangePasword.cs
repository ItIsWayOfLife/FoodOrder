
namespace API.Models.Identity.Users
{
    public class UserModelChangePasword
    {
        public string Id { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
